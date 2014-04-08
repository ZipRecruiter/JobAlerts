/* SubscriberResources.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;

using ZipRecruiter.Schema;

namespace ZipRecruiter.Resource
{
	public class SubscriberResource : APIResource, ICloneable
	{
		public SubscriberResource(JobAlertsAPI handle) : base(handle)
		{
			this.endpoint = "subscriber";
		}

		public override dynamic Retrieve(String subscriberID = "",
		                              String jobSearchID = "",
		                              String batchID = ""){
			if (String.IsNullOrEmpty(subscriberID)) {
				throw new MissingAPIArgumentsException ("subscriber",new ArrayList(){"subscriberID"});
			}

			ArrayList extra = new ArrayList();
			if (!String.IsNullOrEmpty(jobSearchID)){
				extra.Add ("jobSearchID");
			}

			if (!String.IsNullOrEmpty(batchID)) {
				extra.Add ("batchID");
			}

			if (extra.Count > 0){
				throw new ExtraneousAPIArgumentsException ("subscriber",extra);
			}

			base.Retrieve (subscriberID: subscriberID, jobSearchID: jobSearchID);
			return this.Clone();
		}

		public override dynamic Submit(Dictionary<String,String> parameters,
		                              String subscriberID = "",
		                              String jobSearchID = "",
		                              String batchID = "") {
			ArrayList extra = new ArrayList();
			if (!String.IsNullOrEmpty(subscriberID)){
				extra.Add ("subscriberID");
			}

			if (!String.IsNullOrEmpty(jobSearchID)){
				extra.Add ("jobSearchID");
			}

			if (!String.IsNullOrEmpty(batchID)) {
				extra.Add ("batchID");
			}

			if (extra.Count > 0){
				throw new ExtraneousAPIArgumentsException ("subscriber",extra);
			}

			base.Submit (parameters, subscriberID: subscriberID, jobSearchID: jobSearchID);
			return this.Clone();
		}

		public override dynamic Query(Dictionary<String,String> parameters,
		                              String subscriberID = "",
		                              String jobSearchID = "",
		                              String batchID = ""){
			ArrayList extra = new ArrayList();
			if (!String.IsNullOrEmpty(subscriberID)){
				extra.Add ("subscriberID");
			}

			if (!String.IsNullOrEmpty(jobSearchID)){
				extra.Add ("jobSearchID");
			}

			if (!String.IsNullOrEmpty(batchID)) {
				extra.Add ("batchID");
			}

			if (extra.Count > 0){
				throw new ExtraneousAPIArgumentsException ("subscriber",extra);
			}

			base.Query(parameters, subscriberID: subscriberID, jobSearchID: jobSearchID);
			return this.Clone();
		}

		public override dynamic Deactivate(String subscriberID = "",
		                              String jobSearchID = "",
		                              String batchID = ""){
			if (String.IsNullOrEmpty(subscriberID)) {
				throw new MissingAPIArgumentsException ("subscriber",new ArrayList(){"subscriberID"});
			}

			ArrayList extra = new ArrayList();
			if (!String.IsNullOrEmpty(jobSearchID)){
				extra.Add ("jobSearchID");
			}

			if (!String.IsNullOrEmpty(batchID)) {
				extra.Add ("batchID");
			}

			if (extra.Count > 0){
				throw new ExtraneousAPIArgumentsException ("subscriber",extra);
			}

			base.Deactivate(subscriberID: subscriberID, jobSearchID: jobSearchID);
			return this.Clone();
		}

		protected override string BuildRequestUrl ()
		{
			String reqUrl = base.BuildRequestUrl();

			//If we have a subscriber id, it's a more specific request
			if (!String.IsNullOrEmpty (this.subscriberID)) {
				reqUrl += '/' + this.subscriberID;
			}

			//Append Query String parameters if we're doing anything except POSTing
			if (this.operatingMode != APIResourceMode.Submit) {
				reqUrl += this.Parameters.ToQueryString();
			}

			return reqUrl;
		}

		public override dynamic Call ()
		{
			//Abusing dynamic a bit by initializing the WebRequest in the base class
			System.Net.WebRequest request = (System.Net.WebRequest)base.Call();
			String postbody = "";

			if (request.Method.Equals("POST")) {
				//Build post params to send
				//Add the return content-type here since we'll be overriding
				//the one we normally specify in the request
				this.Parameters.Add ("content-type", "text/xml");
				postbody = this.Parameters.ToPostBody();
			} 
			var result = this.handle.MakeRequest (request, this.DetermineSerializationType (), postbody);

			return result;
		}

		protected override Type DetermineSerializationType(){
			if (this.operatingMode == APIResourceMode.Query) {
				return typeof(SubscriberResultSet);
			} else {
				return typeof(SubscriberRecord);
			}
		}
	}
}

