using Castle.Windsor;
using Nancy;
using Nancy.Bootstrappers.Windsor;
using Nancy.Authentication.Forms;
using Nancy.Authentication.Stateless;
using Nancy.Bootstrapper;
using Nancy.Diagnostics;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;

namespace JFTP.Core
{
    public class Bootstrapper : WindsorNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(IWindsorContainer container)
        {
            base.ConfigureApplicationContainer(container);

            // Autoregister everything and set up the user mapper
            // as a singleton instance shared across everything.
            //container.Register(Classes.FromThisAssembly()
            //    .Pick().LifestyleScoped<NancyPerWebRequestScopeAccessor>()
            //    .ConfigureFor<IUserMapper>(cfg => cfg.LifestyleSingleton()));
            /*
             * This bullshit doesn't work and God Almighty may or may not know why.
             * */

            container.Register(Component.For<IUserMapper>().ImplementedBy<UserMapper>());
            /*
             * This, on the other hand, works perfectly.
             * */
        }

        protected override void RequestStartup(IWindsorContainer container, IPipelines pipelines, NancyContext context)
        {
            FormsAuthentication.Enable(pipelines, new FormsAuthenticationConfiguration()
            {
                RedirectUrl = "~/login",
                UserMapper = container.Resolve<IUserMapper>(),
            });

            StatelessAuthentication.Enable(pipelines, new StatelessAuthenticationConfiguration(ctx =>
            {
                if (!ctx.Request.Query.User.HasValue || !ctx.Request.Query.Password.HasValue) return null;

                var userMapper = container.Resolve<IUserMapper>();
                return userMapper.GetUserFromIdentifier(UserMapper.ValidateUser(ctx.Request.Query.User, ctx.Request.Query.Password), ctx);
            }));
        }
    }
}