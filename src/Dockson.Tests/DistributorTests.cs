using Dockson.Domain;
using Dockson.Domain.Projections;
using Dockson.Domain.Transformers;
using Dockson.Domain.Transformers.Build;
using Dockson.Domain.Transformers.Commits;
using Dockson.Domain.Transformers.Deployment;
using Dockson.Tests.Domain;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Dockson.Tests
{
	public class DistributorTests
	{
		private readonly ITestOutputHelper _output;

		public DistributorTests(ITestOutputHelper output)
		{
			_output = output;
		}

		[Fact]
		public void When_handling_a_single_project_pipeline()
		{
			var view = new View();
			var distributor = new Distributor();

			distributor.AddTransformer(new CommitsTransformer());
			distributor.AddTransformer(new BuildTransformer());
			distributor.AddTransformer(new DeploymentTransformer());

			distributor.AddProjection(new MasterIntervalProjection(view.UpdateMasterCommitInterval));
			distributor.AddProjection(new MasterLeadTimeProjection(view.UpdateMasterCommitLeadTime));

			var service = new EventSource(distributor.Project);

			service
				.BranchCommit().Advance(20.Minutes()).MasterCommit()
				.Advance(5.Minutes())
				.BranchCommit().Advance(20.Minutes()).MasterCommit()
				.Advance(5.Minutes())
				.BuildSucceeded()
				.Advance(7.Minutes())
				.ProductionDeployment();

			var day = service.CurrentDay;

			service.ShouldSatisfyAllConditions(
				() => view[service.Name].MasterCommitInterval[day].Median.ShouldBe(25),
				() => view[service.Name].MasterCommitInterval[day].Deviation.ShouldBe(0),
				() => view[service.Name].MasterCommitLeadTime[day].Median.ShouldBe(20),
				() => view[service.Name].MasterCommitLeadTime[day].Deviation.ShouldBe(0)
			);

			_output.WriteLine(JsonConvert.SerializeObject(view, Formatting.Indented));
		}
	}
}
