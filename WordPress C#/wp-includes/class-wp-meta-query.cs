using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

public class MetaQuery
{
    /// <summary>
    /// لیست پرس‌وجوهای متادیتا
    /// </summary>
    public List<MetaQueryClause> Queries { get; private set; } = new();

    /// <summary>
    /// نوع رابطه بین پرس‌وجوها (AND یا OR)
    /// </summary>
    public string Relation { get; set; } = "AND";

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public MetaQuery() { }

    /// <summary>
    /// سازنده کلاس با پارامترهای اولیه
    /// </summary>
    public MetaQuery(List<MetaQueryClause> queries, string relation = "AND")
    {
        Queries = queries;
        Relation = relation;
    }

    /// <summary>
    /// اضافه کردن پرس‌وجو جدید
    /// </summary>
    public void AddQuery(MetaQueryClause query)
    {
        Queries.Add(query);
    }

    /// <summary>
    /// تولید Expression برای پرس‌وجو
    /// </summary>
    public Expression<Func<T, bool>> BuildExpression<T>() where T : class
    {
        if (Queries == null || Queries.Count == 0)
        {
            return x => true;
        }

        Expression<Func<T, bool>> expression = null;

        foreach (var query in Queries)
        {
            var clauseExpression = BuildClauseExpression<T>(query);

            if (expression == null)
            {
                expression = clauseExpression;
            }
            else
            {
                if (Relation.Equals("AND", StringComparison.OrdinalIgnoreCase))
                {
                    expression = CombineExpressions(expression, clauseExpression, Expression.AndAlso);
                }
                else
                {
                    expression = CombineExpressions(expression, clauseExpression, Expression.OrElse);
                }
            }
        }

        return expression;
    }

    /// <summary>
    /// ترکیب دو Expression
    /// </summary>
    private Expression<Func<T, bool>> CombineExpressions<T>(
        Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2,
        Func<Expression, Expression, BinaryExpression> combineOperator)
    {
        var parameter = Expression.Parameter(typeof(T));

        var visitor = new ReplaceParameterVisitor { Parameter = parameter };
        var body1 = visitor.Visit(expr1.Body);
        var body2 = visitor.Visit(expr2.Body);

        return Expression.Lambda<Func<T, bool>>(
            combineOperator(body1, body2),
            parameter
        );
    }

    /// <summary>
    /// ساخت Expression برای یک Clause
    /// </summary>
    private Expression<Func<T, bool>> BuildClauseExpression<T>(MetaQueryClause clause) where T : class
    {
        var parameter = Expression.Parameter(typeof(T));
        Expression expression = null;

        if (!string.IsNullOrEmpty(clause.Key))
        {
            var property = typeof(T).GetProperty(clause.Key);
            if (property != null)
            {
                var propertyAccess = Expression.Property(parameter, property);

                switch (clause.Compare)
                {
                    case "=":
                        expression = Expression.Equal(propertyAccess, Expression.Constant(clause.Value));
                        break;
                    case "!=":
                        expression = Expression.NotEqual(propertyAccess, Expression.Constant(clause.Value));
                        break;
                    case "IN":
                        expression = BuildInExpression(propertyAccess, clause.Value as IEnumerable<object>);
                        break;
                    case "NOT IN":
                        expression = Expression.Not(BuildInExpression(propertyAccess, clause.Value as IEnumerable<object>));
                        break;
                    // موارد دیگر را می‌توان اضافه کرد
                }
            }
        }

        return Expression.Lambda<Func<T, bool>>(expression, parameter);
    }

    /// <summary>
    /// ساخت Expression برای عملگر IN
    /// </summary>
    private Expression BuildInExpression(Expression propertyAccess, IEnumerable<object> values)
    {
        if (values == null || !values.Any())
        {
            return Expression.Constant(false);
        }

        var expressions = values.Select(value => Expression.Equal(propertyAccess, Expression.Constant(value)));
        return expressions.Aggregate(Expression.Or);
    }
}

/// <summary>
/// کلاس برای نگهداری جزئیات یک Clause
/// </summary>
public class MetaQueryClause
{
    public string Key { get; set; }
    public object Value { get; set; }
    public string Compare { get; set; } = "=";
}

/// <summary>
/// کلاس کمکی برای جایگزینی پارامترها
/// </summary>
internal class ReplaceParameterVisitor : ExpressionVisitor
{
    public ParameterExpression Parameter { get; set; }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return Parameter ?? node;
    }
}