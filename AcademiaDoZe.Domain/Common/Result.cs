// Thiago Augusto Ruskowski Waltrick
using System;
using System.Collections.Generic;

namespace AcademiaDoZe.Domain.Common
{
    public class Result<T>
    {
        public T Value { get; }
        public IReadOnlyCollection<Notification> Notifications { get; }
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;

        private Result(T value, IReadOnlyCollection<Notification> notifications, bool isSuccess)
        {
            Value = value;
            Notifications = notifications ?? new List<Notification>();
            IsSuccess = isSuccess;
        }

        public static Result<T> Success(T value) => new Result<T>(value, Array.Empty<Notification>(), true);

        public static Result<T> Failure(IReadOnlyCollection<Notification> notifications) => new Result<T>(default!, notifications ?? new List<Notification>(), false);
    }
}
