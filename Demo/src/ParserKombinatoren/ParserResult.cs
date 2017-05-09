using System;
using System.Linq;

namespace ParserKombinatoren
{
    public abstract class ParserResult<T>
    {
        public static ParserResult<T> Succeed(T value, ParserPosition rest)
        {
            return new Success(value, rest);
        }

        public static ParserResult<T> Failed(string error, ParserPosition position)
        {
            return new Failure(error, position);
        }

        public abstract TRes Match<TRes>(Func<T, ParserPosition, TRes> onSuccess, Func<string, ParserPosition, TRes> onError);
        
        public ParserResult<TRes> Map<TRes>(Func<T, TRes> map)
        {
            return Match((v,r) => ParserResult<TRes>.Succeed(map(v),r), ParserResult<TRes>.Failed);
        }

        public ParserResult<T> OverwriteError(string error)
        {
            return Match(Succeed, (_, pos) => Failed(error, pos));
        }

        private class Success : ParserResult<T>
        {
            internal Success(T value, ParserPosition rest)
            {
                Value = value;
                RestText = rest;
            }

            private T Value { get; }
            private ParserPosition RestText { get; }

            public override TRes Match<TRes>(Func<T, ParserPosition, TRes> onSuccess, Func<string, ParserPosition, TRes> onError)
            {
                return onSuccess(Value, RestText);
            }

        }

        private class Failure : ParserResult<T>
        {
            internal Failure(string error, ParserPosition position)
            {
                Error = error;
                Position = position;
            }

            private string Error { get; }
            private ParserPosition Position { get; }

            public override TRes Match<TRes>(Func<T, ParserPosition, TRes> onSuccess, Func<string, ParserPosition, TRes> onError)
            {
                return onError(Error, Position);
            }
        }
    }
}