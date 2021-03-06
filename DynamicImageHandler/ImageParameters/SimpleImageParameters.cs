﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleImageParameters.cs" company="">
// Copyright (c) 2009-2010 Esben Carlsen
// Forked by Jaben Cargman
//	
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.

// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA  
// </copyright>
// <summary>
//   The simple image parameters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler.ImageParameters
{
	#region Using

	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Cryptography;
	using System.Text;
	using System.Web;

	#endregion

	/// <summary>
	/// 	The simple image parameters.
	/// </summary>
	public class SimpleImageParameters : IImageParameters
	{
		#region Constants and Fields

		/// <summary>
		/// 	The _parameters.
		/// </summary>
		protected SortedDictionary<string, string> _parameters = new SortedDictionary<string, string>();

		#endregion

		#region Public Properties

		/// <summary>
		/// 	Gets ImageSrc.
		/// </summary>
		public virtual string ImageSrc
		{
			get
			{
				if (this.Parameters.ContainsKey("src"))
				{
					return this.Parameters["src"];
				}

				return string.Empty;
			}
		}

		/// <summary>
		/// 	Doesn't cache -- suggested pull once and reuse in local code.
		/// </summary>
		public virtual string Key
		{
			get
			{
				return this.MD5HashString(this.ParametersAsString(), 64);
			}
		}

		/// <summary>
		/// 	Gets Parameters.
		/// </summary>
		public virtual IDictionary<string, string> Parameters
		{
			get
			{
				return this._parameters;
			}
		}

		#endregion

		#region Public Indexers

		/// <summary>
		/// 	The this.
		/// </summary>
		/// <param name = "parameter">
		/// 	The parameter.
		/// </param>
		public virtual string this[string parameter]
		{
			get
			{
				return this.Parameters.ContainsKey(parameter) ? this.Parameters[parameter] : null;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// 	The add collection.
		/// </summary>
		/// <param name="context">
		/// 	The context.
		/// </param>
		public virtual void AddCollection(HttpContext context)
		{
			foreach (string key in context.Request.QueryString.Keys)
			{
				if (this.Parameters.ContainsKey(key))
				{
					this.Parameters[key] = context.Request.QueryString[key];
				}
				else if (!string.IsNullOrEmpty(context.Request.QueryString[key]))
				{
					this.Parameters.Add(key, context.Request.QueryString[key]);
				}
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// 	The m d 5 hash string.
		/// </summary>
		/// <param name="value">
		/// 	The value.
		/// </param>
		/// <param name="maxLength">
		/// 	The max length.
		/// </param>
		/// <returns>
		/// 	The m d 5 hash string.
		/// </returns>
		protected string MD5HashString(string value, int maxLength)
		{
			using (var cryptoServiceProvider = new MD5CryptoServiceProvider())
			{
				byte[] data = Encoding.ASCII.GetBytes(value);
				data = cryptoServiceProvider.ComputeHash(data);

				string str = SymCrypt.BytesToHexString(data);

				if (str.Length > maxLength)
				{
					str = str.Substring(0, maxLength);
				}

				return str;
			}
		}

		/// <summary>
		/// 	The parameters as string.
		/// </summary>
		/// <returns>
		/// 	The parameters as string.
		/// </returns>
		protected virtual string ParametersAsString()
		{
			var builder = new StringBuilder();

			bool isFirst = true;

			// create a key for this item...
			foreach (var kv in this.Parameters.Where(kv => !string.IsNullOrEmpty(kv.Value)))
			{
				if (!isFirst)
				{
					builder.Append("&");
				}

				builder.AppendFormat("{0}={1}", kv.Key.ToLower(), kv.Value);
				isFirst = false;
			}

			return builder.ToString();
		}

		#endregion
	}
}