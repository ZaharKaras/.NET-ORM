using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Lab_6.KarasEF
{
    public class KarasQueryable<T> : IQueryable<T>
    {
        public KarasQueryable(IQueryProvider provider)
        {
            Expression = Expression.Constant(this);
            Provider = provider;
            ElementType = typeof(T);
        }
        public KarasQueryable(Expression expression, KarasQueryProvider provider)
        {
            Expression = expression;
            Provider = provider;
            ElementType = typeof(T);
        }

        public Type ElementType { get; }
        public Expression Expression { get; }
        public IQueryProvider Provider { get; }

        public IEnumerator<T> GetEnumerator()
        {
            return Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
