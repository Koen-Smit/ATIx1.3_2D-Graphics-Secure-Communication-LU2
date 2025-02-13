public class Object2DController : BaseApiController<Object2D>
{
    public Object2DController(IRepository<Object2D> repository) : base(repository) { }
}
public class Environment2DController : BaseApiController<Environment2D>
{
    public Environment2DController(IRepository<Environment2D> repository) : base(repository) { }
}
