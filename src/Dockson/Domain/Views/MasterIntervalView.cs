﻿using System;
using System.Collections.Generic;

namespace Dockson.Domain.Views
{
	public class MasterIntervalView
	{
		public Dictionary<DateTime, Summary> Daily { get; set; }

		public MasterIntervalView()
		{
			Daily = new Dictionary<DateTime, Summary>();
		}
	}
}
