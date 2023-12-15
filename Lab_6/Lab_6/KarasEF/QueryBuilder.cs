using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Lab_6.KarasEF
{
    public class QueryBuilder : ExpressionVisitor
    {
        private Expression selectList, whereExpression, joinExpression;
        private readonly IKarasContext _context;
        public QueryBuilder(IKarasContext context)
        {
            _context = context;
        }
        
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if(node.Method.IsGenericMethod)
            {
                var genericMethod = node.Method.GetGenericMethodDefinition();

                if (genericMethod == QueryableMethods.Select)
                {
                    VisitSelect(node);
                }
                else if (genericMethod == QueryableMethods.Where)
                {
                    VisitWhere(node);
                }
                else if (genericMethod == QueryableMethods.Join)
                {
                    VisitJoin(node);
                }
                
               
            }
            
            return base.VisitMethodCall(node);
        }

        private void VisitWhere(MethodCallExpression node)
        {
            whereExpression = ((UnaryExpression)node.Arguments[1]).Operand;
        }

        private void VisitSelect(MethodCallExpression node)
        {
            selectList = ((UnaryExpression)node.Arguments[1]).Operand;
        }
        private void VisitJoin(MethodCallExpression node)
        {
            joinExpression = node.Arguments[2]; 
        }


        internal FormattableString Compile(Expression expression)
        {
            Visit(expression);
            var whereVisitor = new WhereVisitor();
            whereVisitor.Visit(whereExpression);
            var selectVisitor = new SelectVisitor();
            selectVisitor.Visit(selectList);
            var whereClause = whereVisitor.Result;
            var selectClause = selectVisitor.Result;
            var joinVisitor = new JoinVisitor();
            joinVisitor.Visit(joinExpression);
            var joinClause = joinVisitor.Result;
            var tableName = expression.Type.GenericTypeArguments[0].Name.ToLower();

            if (whereClause is null)
            {
                if (joinClause is null)
                {
                    return FormattableStringFactory.Create($"select {selectClause.ToLower()} from \"{tableName}\"");
                }
                else
                {
                    return FormattableStringFactory.Create($"select {selectClause.ToLower()} from \"{tableName}\" {joinClause.ToLower()}");
                }
            }
            else
            {
                if (joinClause is null)
                {
                    return FormattableStringFactory.Create($"select {selectClause.ToLower()} from \"{tableName}\" where {whereClause.ToLower()}");
                }
                else
                {
                    return FormattableStringFactory.Create($"select {selectClause.ToLower()} from \"{tableName}\" {joinClause.ToLower()} where {whereClause.ToLower()}");
                }
            }
        }
    }

    internal class StringExpression : Expression
    {
        public string String { get; set; }

        public StringExpression(string @string, ExpressionType nodeType, Type type)
        {
            String = @string;
            NodeType = nodeType;
            Type = type;
        }

        public override ExpressionType NodeType { get; }
        public override Type Type { get; }
    }

    internal class WhereVisitor: ExpressionVisitor
    {
        protected override Expression VisitBinary(BinaryExpression node)
        {
            var @operator = node.NodeType switch
            {
                ExpressionType.GreaterThan => ">",
                ExpressionType.LessThan => "<",
                ExpressionType.Equal => "=",
                ExpressionType.OrElse => "or",
                ExpressionType.AndAlso => "and",
                ExpressionType.LessThanOrEqual => "=<",
                ExpressionType.GreaterThanOrEqual => "=>",
                ExpressionType.NotEqual => "!="
            };

            var left = ToString(node.Left);
            var right = ToString(node.Right);
            
            Result = $"{left} {@operator} {right}";
            return base.VisitBinary(node);
        }

        public string Result { get; set; }

        private string ToString(Expression expression)
        {
            if (expression is ConstantExpression constantExpression)
                return constantExpression.Value.ToString();
            return $"{((MemberExpression)expression).Member.Name}";
        }
    }

    internal class JoinVisitor : ExpressionVisitor
    {
        public string Result { get; private set; }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "Join")
            {
                var outer = ToString(node.Arguments[0]);
                var inner = ToString(node.Arguments[1]);
                var outerKey = ToString(((LambdaExpression)((UnaryExpression)node.Arguments[2]).Operand).Body);
                var innerKey = ToString(((LambdaExpression)((UnaryExpression)node.Arguments[3]).Operand).Body);

                Result = $"join {inner} on {outerKey} equals {innerKey}";
            }

            return base.VisitMethodCall(node);
        }

        private string ToString(Expression expression)
        {
            if (expression is ConstantExpression constantExpression)
                return constantExpression.Value.ToString();
            return $"{((MemberExpression)expression).Member.Name}";
        }
    }



    internal class SelectVisitor : ExpressionVisitor
    {
        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            var nodes = node.Bindings.Cast<MemberAssignment>().Select(x => ToString(x.Expression));
            Result = string.Join(", ", nodes);
            return base.VisitMemberInit(node);
        }

        public string Result { get; set; }

        private string ToString(Expression expression)
        {
            if (expression is ConstantExpression constantExpression)
                return constantExpression.Value.ToString();
            return $"{((MemberExpression)expression).Member.Name}";
        }
    }
    
}
