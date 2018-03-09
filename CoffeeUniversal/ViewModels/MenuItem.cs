using System;

namespace CoffeeUniversal.ViewModels
{
	public class MenuItem
	{
		private string name;
		public string Name
		{
			get { return name; }
			set
			{
				if (name != value)
				{
					name = value;
				}
			}
		}

		private string content;
		public string Content
		{
			get { return content; }
			set
			{
				if (content != value)
				{
					content = value;
				}
			}
		}

		private Type pageType;
		public Type PageType
		{
			get { return pageType; }
			set
			{
				if (pageType != value)
				{
					pageType = value;
				}
			}
		}

		private bool isEnabled = true;
		public bool IsEnabled
		{
			get { return isEnabled; }
			set { isEnabled = value; }
		}

	}
}
