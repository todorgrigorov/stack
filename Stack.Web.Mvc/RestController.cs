using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stack.Data;
using Stack.Persistence;

namespace Stack.Web.Mvc
{
    [Route("[controller]")]
    public abstract class RestController : Controller
    {
        #region Protected members
        protected async Task<IActionResult> NotImplemented()
        {
            return await Task.FromResult(StatusCode((int)HttpStatusCode.NotImplemented));
        }
        protected BadRequestObjectResult InvalidIdResult(int id)
        {
            return BadRequest(new InvalidIdentifierResult("Invalid identifier.", id));
        }

        protected PageOptions ValidatePageOption(PageOptions page)
        {
            if (page != null && !page.IsValid())
            {
                page = null;
            }
            return page;
        }
        protected SortOptions[] ValidateSortOption(SortOptions[] sort)
        {
            if ((sort != null && sort.Length == 0) ||
                    (sort != null && sort.Length == 1 && sort[0] == null))
            {
                sort = null;
            }
            return sort;
        }
        #endregion
    }

    public abstract class ReadonlyRestController<TEntity, TModel, TFilter> : RestController
        where TEntity : Entity, new()
        where TModel : Model, new()
        where TFilter : Filter
    {
        public ReadonlyRestController()
        {
            ContractDao = Dao.ForContract<TEntity, TModel>();
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Get(int id)
        {
            IActionResult result = null;
            if (id <= 0)
            {
                result = InvalidIdResult(id);
            }
            else
            {
                try
                {
                    TModel model = ContractDao.Load(id);
                    result = Ok(model);
                }
                catch (EntityNotFoundException e)
                {
                    result = NotFound(e.Error);
                }
            }
            return await Task.FromResult(result);
        }
        [HttpGet]
        public virtual async Task<IActionResult> Get(
            [FromQuery]TFilter filter = null,
            [FromQuery]PageOptions page = null,
            [FromQuery]SortOptions[] sort = null)
        {
            page = ValidatePageOption(page);
            sort = ValidateSortOption(sort);

            return await Task.FromResult(Ok(new ListResult<TModel>(
                ContractDao.Count(filter),
                ContractDao.List(filter: filter, page: page, sort: sort)
            )));
        }

        #region Protected members
        protected IDao<TModel> ContractDao { get; private set; }
        #endregion
    }

    public abstract class RestController<TEntity, TModel, TFilter> : ReadonlyRestController<TEntity, TModel, TFilter>
        where TEntity : Entity, new()
        where TModel : Model, new()
        where TFilter : Filter
    {
        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody]TModel data)
        {
            IActionResult result = null;
            if (!data.IsNew)
            {
                result = InvalidIdResult(data.Id);
            }
            else
            {
                result = CreateOrUpdate(data, CrudOperation.Create);
            }
            return await Task.FromResult(result);
        }
        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Put(int id, [FromBody]TModel data)
        {
            IActionResult result = null;
            if (data.IsNew || data.Id < 0)
            {
                result = InvalidIdResult(data.Id);
            }
            else
            {
                result = CreateOrUpdate(data, CrudOperation.Update);
            }
            return await Task.FromResult(result);
        }
        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            IActionResult result = null;
            if (id <= 0)
            {
                result = InvalidIdResult(id);
            }
            else
            {
                TModel model = null;
                Database.InTransaction(() =>
                {
                    try
                    {
                        model = ContractDao.Delete(id);
                        result = Ok(model);
                    }
                    catch (EntityNotFoundException e)
                    {
                        result = NotFound(e.Error);
                    }
                });
            }
            return await Task.FromResult(result);
        }

        #region Private members
        private IActionResult CreateOrUpdate(TModel data, CrudOperation crud)
        {
            IActionResult result = null;

            TModel model = null;
            Database.InTransaction(() =>
            {
                try
                {
                    if (crud == CrudOperation.Create)
                    {
                        model = ContractDao.Create(data);
                        result = new CreatedResult(Request.Path, model);
                    }
                    else
                    {
                        model = ContractDao.Update(data);
                        result = Ok(model);
                    }
                }
                catch (EntityNotFoundException e)
                {
                    result = NotFound(e.Error);
                }
                catch (EntityNotUniqueException e)
                {
                    result = BadRequest(e.Error);
                }
                catch (ValidationException e)
                {
                    result = BadRequest(e.Error);
                }
            });

            return result;
        }
        #endregion
    }
}