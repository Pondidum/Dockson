using System;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;

namespace Dockson
{
	public class HomeController : Controller
	{
		[Route("")]
		public IActionResult Index()
		{
			return Ok();
		}
	}

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

			viewStore.Load();
			stateStore.Load();

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

			services.AddSingleton<IViewStore>(viewStore);
			services.AddSingleton<IProjector>(new PreProjector(new ValidationProjector(dist)));

			services
				.AddMvcCore(c => c.Filters.Add<NotificationValidationFilter>())
				.AddJsonFormatters();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMvc();
		}
	}
}
