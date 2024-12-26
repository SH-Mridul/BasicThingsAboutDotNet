using Autofac;
using Practice.Models.Item;

namespace Practice
{
    public class WebModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Item>().As<IItem>().SingleInstance();
            //builder.RegisterType<Item>().As<IItem>().InstancePerLifetimeScope();
            //builder.RegisterType<Item>().AsSelf().SingleInstance(); //if we not using any interface
            base.Load(builder);
        }
    }
}
