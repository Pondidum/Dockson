using System;
using System.Collections.Generic;
using Dockson.Domain;
using Dockson.Domain.Projections;
using Dockson.Domain.Projections.MasterInterval;
using Dockson.Domain.Projections.MasterLeadTime;
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
			var distributor = new Distributor(new ITransformer[]
			{
				new CommitsTransformer(),
				new BuildTransformer(),
				new DeploymentTransformer()
			});

			var masterInterval = new IntervalView();
			var masterLeadTime = new LeadTimeView();

			distributor.AddProjection(new MasterIntervalProjection(masterInterval));
			distributor.AddProjection(new MasterLeadTimeProjection(masterLeadTime));

			var service = new EventSource(distributor.Project);

			service
				.BranchCommit()
				.Advance(20.Minutes())
				.MasterCommit()
				.Advance(5.Minutes())
				.BuildSucceeded()
				.Advance(7.Minutes())
				.ProductionDeployment();

			var date = new DayDate(service.Timestamp);

			service.ShouldSatisfyAllConditions(
				() => masterInterval[service.Name].Daily[date].Median.ShouldBe(0),
				() => masterInterval[service.Name].Daily[date].Deviation.ShouldBe(0),
				() => masterLeadTime[service.Name].Daily[date].Median.ShouldBe(20),
				() => masterLeadTime[service.Name].Daily[date].Deviation.ShouldBe(0)
			);
		}
	}
}
