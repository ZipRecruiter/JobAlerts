/* Batch.cs
 * 
 * Copyright 2013 ZipRecruiter Inc
 * ZipRecruiter, Inc.
 * 1453 Third Street Promenade, Suite 335
 * Santa Monica, CA 90401
 * 
 * Author: Maxwell Cabral
 * max@ziprecruiter.com
 */
using System;
using ZipRecruiter.Resource;

namespace ZipRecruiter.Schema {

	/// <summary>
	/// Immutable Batch result set.
	/// </summary>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17020")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlRoot("batches")]
	public partial class BatchResultSet : IZipQueryable{

		internal BatchResultSet () {}

		/// <summary>
		/// The maximum number of results returned by the API call
		/// </summary>
		/// <value>The limit.</value>
		[System.Xml.Serialization.XmlElementAttribute("limit")]
		public int Limit { get; set; }

		/// <summary>
		/// The offset of the result set.
		/// </summary>
		/// <value>The offset.</value>
		[System.Xml.Serialization.XmlElementAttribute("offset")]
		public int Offset { get; set; }

		/// <summary>
		/// The number of Result records returned
		/// </summary>
		/// <value>The total_count.</value>
		[System.Xml.Serialization.XmlElementAttribute("total_count")]
		public int TotalCount { get; set; }

		/// <summary>
		/// The result records retuned by the query.
		/// </summary>
		/// <value>The results.</value>
		[System.Xml.Serialization.XmlArray("results")]
		public BatchRecord[] Results { get; set; }
	}

	/// <summary>
	/// Immutable Batch result.
	/// </summary>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17020")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlType("batch")]
	public partial class BatchRecord {

		internal BatchRecord () {}

		/// <summary>
		/// Job Search ID of the record.
		/// </summary>
		/// <value>The identifier.</value>
		[System.Xml.Serialization.XmlElementAttribute("id")]
		public string ID { get; set; }

		/// <summary>
		/// When the record was entered into the system.
		/// </summary>
		/// <value>The create time.</value>
		[System.Xml.Serialization.XmlIgnore]
		public DateTime CreateTime { get; set; }

		/// <summary>
		/// When the record was entered into the system
		/// </summary>
		/// <value>The create_time.</value>
		public string create_time {
			get {
				return this.CreateTime.ToString("o");
			}
			set {
				this.CreateTime = DateTime.Parse (value);
			}
		}

		/// <summary>
		/// The status of the Batch process
		/// </summary>
		/// <value>The deactivation_reason.</value>
		[System.Xml.Serialization.XmlElementAttribute("status")]
		public string Status { get; set; }


		/// <summary>
		/// The type of the Batch process
		/// </summary>
		/// <value>The deactivation_reason.</value>
		[System.Xml.Serialization.XmlElementAttribute("type")]
		public string Type { get; set; }

		/// <summary>
		/// Gets the deactivation time.
		/// </summary>
		/// <value>The deactivation time.</value>
		[System.Xml.Serialization.XmlIgnore]
		public DateTime? CompleteTime { get; set; }

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("complete_time")]
		public string complete_time {
			get {
				if (this.CompleteTime != null) {
					return ((DateTime)this.CompleteTime).ToString("o");
				}
				return null;
			}
			set {
				if (!String.IsNullOrEmpty(value)) {
					this.CompleteTime = (DateTime?)DateTime.Parse (value);
				}
			}
		}
	}
}

