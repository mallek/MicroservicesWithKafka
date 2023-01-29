using CQRS.Core.Infrastructure;
using CQRS.Core.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Infrastructure.Dispatchers
{
    public class QueryDispatcher : IQueryDispatcher<PostEntity>
    {
        private readonly Dictionary<Type, Func<BaseQuery, Task<List<PostEntity>>>> _handlers = new();

        public void RegisterHandler<TQuery>(Func<TQuery, Task<List<PostEntity>>> handler) where TQuery : BaseQuery
        {
            if (_handlers.ContainsKey(typeof(TQuery)))
            {
                throw new IndexOutOfRangeException($"Handler for {typeof(TQuery).Name} is already registered");
            }
            _handlers.Add(typeof(TQuery), query => handler((TQuery)query));
        }

        public async Task<List<PostEntity>> SendAsync<TQuery>(BaseQuery query)
        {
            if (!_handlers.ContainsKey(query.GetType()))
            {
                throw new ArgumentNullException($"Handler for {query.GetType().Name} is not registered");
            }
            
            return await _handlers[query.GetType()](query);


        //    if(_handlers.TryGetValue(query.GetType(), out Func<BaseQuery, Task<List<PostEntity>>>? handler))
        //    {
        //        return await handler(query);
        //    }

        }
    }
}