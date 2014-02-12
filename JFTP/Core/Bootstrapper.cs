using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Authentication.Stateless;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace JFTP.Core
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);
            container.Register<IUserMapper, UserMapper>();
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
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