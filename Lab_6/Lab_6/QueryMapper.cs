using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lab_6
{
    public static class QueryMapper
    {
        private static readonly MethodInfo
            GetStrignMethod = typeof(DataReaderExtensions).GetMethod("GetString")!,
            GetInt32Method = typeof(DataReaderExtensions).GetMethod("GetInt32")!,
            GetInt32NullableMethod = typeof(DataReaderExtensions).GetMethod("GetInt32Nullable")!,
            GetDateTimeMethod = typeof(DataReaderExtensions).GetMethod("GetDateTime")!,
            GetNullableDateTimeMethod = typeof(DataReaderExtensions).GetMethod("GetNullableDateTime")!;


        private static ConcurrentDictionary<Type, Delegate> _mapperFuncs = new();

        public static async Task<List<T>> QueryAsync<T>(this NpgsqlConnection connection, FormattableString sql, CancellationToken cancellationToken)
        {
            await using var command = connection.CreateCommand();

            command.CommandText = ReplaceParameters(sql);

            for (int i = 0; i < sql.ArgumentCount; i++)
            {
                command.Parameters.AddWithValue($"@p{i}", sql.GetArgument(i));
            }

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            var list = new List<T>();
            Func<IDataReader, T> func = (Func<IDataReader, T>)_mapperFuncs.GetOrAdd(typeof(T), x => Build<T>());


            while (await reader.ReadAsync(cancellationToken))
            {
                list.Add(func(reader));
            }

            return list;
        }

        public static async Task<IList> QueryAsyncType(this NpgsqlConnection connection, FormattableString sql, Type entityType, CancellationToken cancellationToken)
        {
            var result = (Task)typeof(QueryMapper).GetMethod("QueryAsync")
                .MakeGenericMethod(entityType)
                .Invoke(null, new Object[] {connection, sql, cancellationToken});

            await result;
            dynamic r = result;
            return r.Result;
        }

        private static string ReplaceParameters(FormattableString sql)
        {
            var arguments = sql.GetArguments();
            var query = string.Format(sql.Format, arguments);
            return query;
        }



        private static Func<IDataReader, T> Build<T>()
        {
            var readerParam = Expression.Parameter(typeof(IDataReader));

            var newExp = Expression.New(typeof(T));
            var memberInit = Expression.MemberInit(newExp, typeof(T).GetProperties()
                .Select(x => Expression.Bind(x, BuildReadColumnExpression(readerParam, x))));

            return Expression.Lambda<Func<IDataReader, T>>(memberInit, readerParam).Compile();
        }

        private static Expression BuildReadColumnExpression(Expression reader, PropertyInfo prop)
        {
            if (prop.PropertyType == typeof(string))
                return Expression.Call(null, GetStrignMethod, reader, Expression.Constant(prop.Name.ToLower()));
            else if (prop.PropertyType == typeof(int?))
                return Expression.Call(null, GetInt32NullableMethod, reader, Expression.Constant(prop.Name.ToLower()));
            else if (prop.PropertyType == typeof(int))
                return Expression.Call(null, GetInt32Method, reader, Expression.Constant(prop.Name.ToLower()));
            else if (prop.PropertyType == typeof(DateTime))
                return Expression.Call(null, GetDateTimeMethod, reader, Expression.Constant(prop.Name.ToLower()));
            else if (prop.PropertyType == typeof(DateTime?))
                return Expression.Call(null, GetNullableDateTimeMethod, reader, Expression.Constant(prop.Name.ToLower()));


            throw new InvalidOperationException();
        }
    }
}
