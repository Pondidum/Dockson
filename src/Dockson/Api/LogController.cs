using Dockson.Domain;
using Dockson.Domain.Transformers.Build;
using Dockson.Domain.Transformers.Commits;
using Dockson.Domain.Transformers.Deployment;
using Microsoft.AspNetCore.Mvc;

namespace Dockson.Api
{
	[Route("api/[controller]/[action]")]
	public class LogController : Controller
	{
		private readonly IProjector _projector;

		public LogController(IProjector projector)
		{
			_projector = projector;
		}

		[HttpPost]
		public IActionResult Commit([FromBody]CommitNotification model)
		{
			_projector.Project(model);
			return Ok();
		}

		[HttpPost]
		public IActionResult Build(BuildNotification model)
		{
			_projector.Project(model);
			return Ok();
		}

		[HttpPost]
		public IActionResult Deployment(DeploymentNotification model)
		{
			_projector.Project(model);
			return Ok();
		}

		public IActionResult Incident() => Ok();
	}
}
