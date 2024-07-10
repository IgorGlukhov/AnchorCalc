using AnchorCalc.Domain.Factories;
using Autofac;

namespace AnchorCalc.Bootstrapper.Factories;

public class Factory<TResult>(IComponentContext componentContext) : IFactory<TResult>
{
    public TResult Create()
    {
        var factory = componentContext.Resolve<Func<TResult>>();
        return factory.Invoke();
    }
}