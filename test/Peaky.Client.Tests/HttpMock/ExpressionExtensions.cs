﻿using System;
using System.Linq.Expressions;

namespace Peaky.Client.Tests.HttpMock.FilterBuilders;

internal static class ExpressionExtensions
{
    public static Expression<Func<TFirstParam, TResult>>
        Compose<TFirstParam, TIntermediate, TResult>(
            this Expression<Func<TFirstParam, TIntermediate>> first,
            Expression<Func<TIntermediate, TResult>> second, string parameterName = null)
    {
        var param = Expression.Parameter(typeof(TFirstParam), parameterName ?? "param");

        var newFirst = first.Body.Replace(first.Parameters[0], param);
        var newSecond = second.Body.Replace(second.Parameters[0], newFirst);

        return Expression.Lambda<Func<TFirstParam, TResult>>(newSecond, param);
    }

    public static Expression Replace(this Expression expression,
        Expression searchEx, Expression replaceEx)
    {
        return new ReplaceVisitor(searchEx, replaceEx).Visit(expression);
    }

    internal class ReplaceVisitor : ExpressionVisitor
    {
        private readonly Expression _from, _to;
        public ReplaceVisitor(Expression from, Expression to)
        {
            _from = from;
            _to = to;
        }
        public override Expression Visit(Expression node)
        {
            return node == _from ? _to : base.Visit(node);
        }
    }
}