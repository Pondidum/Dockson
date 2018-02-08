using System.Net;
using System.Threading.Tasks;
using Dockson.Domain;
using Dockson.Domain.Projections;
using Dockson.Domain.Transformers.Build;
using Dockson.Domain.Transformers.Commits;
using Dockson.Domain.Transformers.Deployment;
using Dockson.Infrastructure;
using Dockson.Infrastructure.Validation;
using Dockson.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dockson
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			var settings = new Settings
			{
				StoragePath = "./store"
			};

			var fs = new FileSystem();
			fs.CreateDirectory(settings.StoragePath);

			var viewStore = new ViewStore(fs, settings);
			var stateStore = new StateStore(fs, settings);
			var notificationStore = new NotificationStore(fs, settings);

			viewStore.Load();
			stateStore.Load();

			var dist = BuildDistributor(stateStore, viewStore);
			var projector = new SequencingProjector(new NotificationWriter(notificationStore, dist));

			services.AddSingleton<IViewStore>(viewStore);
			services.AddSingleton<IProjector>(new PreProjector(new ValidationProjector(projector)));
			services.AddSingleton<IHostedService>(projector);

			services
				.AddMvcCore(c => c.Filters.Add<NotificationValidationFilter>())
				.AddJsonFormatters()
				.AddRazorViewEngine();
		}

		private static Distributor BuildDistributor(StateStore stateStore, ViewStore viewStore)
		{
			var dist = new Distributor(stateStore, viewStore);

			dist.AddTransformer(new CommitsTransformer());
			dist.AddTransformer(new BuildTransformer());
			dist.AddTransformer(new DeploymentTransformer());

			dist.AddProjection(new MasterLeadTimeProjection(viewStore.UpdateMasterCommitLeadTime));
			dist.AddProjection(new MasterIntervalProjection(viewStore.UpdateMasterCommitInterval));
			dist.AddProjection(new BuildLeadTimeProjection(viewStore.UpdateBuildLeadTime));
			dist.AddProjection(new BuildIntervalProjection(viewStore.UpdateBuildInterval));
			dist.AddProjection(new BuildRecoveryTimeProjection(viewStore.UpdateBuildRecoveryTime));
			dist.AddProjection(new BuildFailureRateProjection(viewStore.UpdateBuildFailureRate));
			dist.AddProjection(new DeploymentLeadTimeProjection(viewStore.UpdateDeploymentLeadTime));
			dist.AddProjection(new DeploymentIntervalProjection(viewStore.UpdateDeploymentInterval));

			return dist;
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
				{
					HotModuleReplacement = true,
					ReactHotModuleReplacement = true
				});
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStaticFiles();
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");

				routes.MapSpaFallbackRoute(
					name: "spa-fallback",
					defaults: new { controller = "Home", action = "Index" });
			});

			app.Run(context =>
			{
				context.Response.StatusCode = (int)HttpStatusCode.NotFound;
				return Task.CompletedTask;
			});
		}
	}
}
