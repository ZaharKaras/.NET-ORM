using Lab_6.Entities;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Lab_6.KarasEF
{
    public class KarasSet<T> : KarasQueryable<T>
    {
        private KarasQueryProvider queryProvider;
        public KarasSet(IQueryProvider provider) : base(provider)
        {
            queryProvider = (KarasQueryProvider?)provider;
        }

        public async Task AddAsync(T entity)
        {
            var tableName = typeof(T).Name.ToLower();
            var properties = typeof(T).GetProperties()
                                          .Select(prop => prop.Name)
                                          .ToList();

            var values = string.Join(", ", properties.Select(prop =>
            {
                var propValue = typeof(T).GetProperty(prop)?.GetValue(entity);
                if (propValue != null)
                {
                    if (propValue is int || propValue is double || propValue is float || propValue is decimal)
                    {
                        return propValue.ToString();
                    }
                    return $"'{propValue.ToString()}'";
                }
                return "NULL";
            }));

            FormattableString sql = $"""
            insert into "{tableName}"
            values
            ({values});
            """
            ;

            var connection = queryProvider.GetConnection();

            try
            {
                await connection.QueryAsync<T>(sql, default);
                Console.WriteLine("Insert successful!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting data: {ex.Message}");
            }


        }

        public async Task DeleteAsync(string entityName)
        {
            var tableName = typeof(T).Name.ToLower();

            FormattableString sql = $"""
            delete from "{tableName}"
            where "name" = '{entityName}';
            """
            ;

            var connection = queryProvider.GetConnection();

            try
            {
                await connection.QueryAsync<T>(sql, default);
                Console.WriteLine("Insert successful!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting data: {ex.Message}");
            }

        }

        public async Task UpdateAsync(T entity, int id)
        {
            var entityType = typeof(T);
            var properties = entityType.GetProperties();
            var keyProperty = properties.FirstOrDefault(prop => prop.Name.ToLower().EndsWith("_id"));

            if (keyProperty == null)
            {
                Console.WriteLine("Entity doesn't contain a key property.");
                return;
            }

            var tableName = entityType.Name.ToLower();
            var keyValue = keyProperty.GetValue(entity);

            var setValues = string.Join(", ", properties
                .Where(prop => prop != keyProperty)
                .Select(prop =>
                {
                    var propValue = prop.GetValue(entity);
                    return $"{prop.Name} = {FormatValue(propValue)}";
                }));

            FormattableString sql = $"""
            update "{tableName}"
            set {setValues}
            where "{keyProperty.Name.ToLower()}" = {FormatValue(keyValue)};
            """;

            var connection = queryProvider.GetConnection();

            try
            {
                await connection.QueryAsync<T>(sql, default);
                Console.WriteLine("Update successful!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating data: {ex.Message}");
            }
        }

        private string FormatValue(object? value)
        {
            if (value == null)
            {
                return "NULL";
            }
            else if (value is int || value is double || value is float || value is decimal)
            {
                return value.ToString();
            }
            else
            {
                return $"'{value.ToString()}'";
            }
        }


    }
}
