using Dockson.Domain;
using Dockson.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Dockson.Api
{
	[Route("api/[controller]")]
	public class ViewController : Controller
	{
		private readonly IViewStore _viewStore;

		public ViewController(IViewStore viewStore)
		{
			_viewStore = viewStore;
		}

		[HttpGet]
		public JsonResult Get()
		{
			return new JsonResult(_viewStore.View);
		}

		[HttpGet("groups")]
		public JsonResult Groups()
		{
			return new JsonResult(_viewStore.View.Keys);
		}

		[HttpGet("groups/{group}")]
		public IActionResult Groups(string group)
		{
			if (_viewStore.View.ContainsKey(group) == false)
				return NotFound();

			return new JsonResult(_viewStore.View[group]);
		}
	}
}
