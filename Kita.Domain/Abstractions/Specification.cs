using System.Linq.Expressions;

namespace Kita.Domain.Abstractions
{
    public abstract class Specification<T>
    {
        public Expression<Func<T, bool>>? Criteria { get; protected set; }
        public List<Expression<Func<T, object>>> Includes { get; } = new();

        public Expression<Func<T, object>>? OrderBy { get; protected set; }
        public Expression<Func<T, object>>? OrderByDescending { get; protected set; }

        public int? Take { get; protected set; }
        public int? Skip { get; protected set; }

        public bool IsPagingEnabled { get; protected set; }
    }

}
