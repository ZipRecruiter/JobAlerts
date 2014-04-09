/* Subscriber.cs
 * 
 * Copyright 2013-2014 ZipRecruiter Inc
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
	/// Immutable Subscriber result set.
	/// </summary>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17020")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlRoot("subscribers")]
	public partial class SubscriberResultSet : IZipQueryable{

		internal SubscriberResultSet () {}

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
		public SubscriberRecord[] Results { get; set; }

		public void SetRelationData(JobAlertsAPI handle){
			/*foreach (SubscriberResult res in this.Results) {
				res.APIHandle = handle;
			}*/
		}
	}

	/// <summary>
	/// Immutable Subscriber result.
	/// </summary>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17020")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlType("subscriber")]
	public partial class SubscriberRecord {

		internal SubscriberRecord () {}

		/// <summary>
		/// The MD5 hash of the subscriber's email
		/// </summary>
		/// <value>The email M d5.</value>
		[System.Xml.Serialization.XmlElementAttribute("email_md5")]
		public string EmailMD5 { get; set; }

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
		/// If the record has been deactivated, the stated reason why it was deactivated.
		/// </summary>
		/// <value>The deactivation_reason.</value>
		[System.Xml.Serialization.XmlElementAttribute("deactivation_reason")]
		public string DeactivationReason { get; set; }

		/// <summary>
		/// When the record was deactivated.
		/// </summary>
		/// <value>The deactivation time.</value>
		[System.Xml.Serialization.XmlIgnore]
		public DateTime? DeactivationTime { get; set; }

		/// <summary>
		/// When the record was deactivated.
		/// </summary>
		/// <value>The deactivation_time.</value>
		public string deactivation_time {
			get {
				if (this.DeactivationTime != null) {
					return ((DateTime)this.DeactivationTime).ToString("o");
				}
				return null;
			}
			set {
				if (!String.IsNullOrEmpty(value)) {
					this.DeactivationTime = (DateTime?)DateTime.Parse (value);
				}
			}
		}

		/// <summary>
		/// The MD5 hash of the subscriber's email
		/// </summary>
		/// <value>The email M d5.</value>
		[System.Xml.Serialization.XmlElementAttribute("brand")]
		public string Brand { get; set; }

	}
}

