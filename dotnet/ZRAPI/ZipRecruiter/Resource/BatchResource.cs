/* BatchResources.cs
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
using System.IO;

namespace ZipRecruiter.Resource
{
	public class BatchResource : APIResource, ICloneable
	{
		public BatchResource(JobAlertsAPI handle) : base(handle)
		{
			this.endpoint = "batch";
		}

		public override dynamic Retrieve(String subscriberID = "",
		                                 String jobSearchID = "",
		                                 String batchID = ""){
			if (String.IsNullOrEmpty(batchID)) {
				throw new MissingAPIArgumentsException ("batch",new ArrayList(){"batchID"});
			}

			ArrayList extra = new ArrayList();
			if (!String.IsNullOrEmpty(jobSearchID)){
				extra.Add ("jobSearchID");
			}

			if (!String.IsNullOrEmpty(subscriberID)) {
				extra.Add ("subscriberID");
			}

			if (extra.Count > 0){
				throw new ExtraneousAPIArgumentsException ("batch",extra);
			}

			base.Retrieve(batchID:batchID);
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
				throw new ExtraneousAPIArgumentsException ("batch",extra);
			}

			base.Submit (parameters, batchID:batchID);
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
				throw new ExtraneousAPIArgumentsException ("batch",extra);
			}

			base.Query(parameters, batchID:batchID);
			return this.Clone();
		}

		/// <summary>
		/// Not available for Batch requests
		/// </summary>
		/// <param name="subscriberID">Subscriber I.</param>
		/// <param name="jobSearchID">Job search I.</param>
		/// <param name="batchID">Batch I.</param>
		/// <returns>If found, the correct Result record for the specified type.</returns>
		public override dynamic Deactivate(String subscriberID = "",
		                                   String jobSearchID = "",
		                                   String batchID = ""){
			throw new NotImplementedException ("Deactivating Batch requests is currently not supported");
		}

		protected override string BuildRequestUrl ()
		{
			String reqUrl = base.BuildRequestUrl();

			//If we have a subscriber id, it's a more specific request
			if (!String.IsNullOrEmpty (this.batchID)) {
				reqUrl += '/' + this.batchID;
			}

			//Append Query String parameters if we're doing anything except POSTing
			if (this.operatingMode != APIResourceMode.Submit) {
				reqUrl += this.Parameters.ToQueryString();
			}

			return reqUrl;
		}

		public override dynamic Call () {
			//Abusing dynamic a bit by initializing the WebRequest in the base class
			System.Net.WebRequest request = (System.Net.WebRequest)base.Call();
			String body = null;
			dynamic result;

			if (request.Method.Equals("POST")) {
				//Build post params to send
				//Add the return content-type here since we'll be overriding
				//the one we normally specify in the request
				this.Parameters.Add ("content-type", "text/xml");

				String filename = this.Parameters["content"];
				this.Parameters.Remove("content");

				System.IO.FileStream upload = System.IO.File.OpenRead(filename);

				string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
				string contentType = "multipart/form-data; boundary=" + formDataBoundary;

				request.ContentType = contentType;

				Dictionary<String,Object> postParameters = new Dictionary<string, object>();

				foreach (KeyValuePair<String,String> p in this.Parameters) {
					postParameters.Add (p.Key, p.Value);
				}

				ZipRecruiter.FormUpload.FileParameter file = new FormUpload.FileParameter(
					new BinaryReader (upload).ReadBytes((int)upload.Length));
				file.ContentType = "text/csv";
				file.FileName = upload.Name;

				postParameters.Add ("content", file);

				byte[] formData = ZipRecruiter.FormUpload.GetMultipartFormData(postParameters, formDataBoundary);

				result = this.handle.MakeRequest(request, this.DetermineSerializationType (), formData);
			} else {
				result = this.handle.MakeRequest(request, this.DetermineSerializationType (), body);
			}

			return result;
		}

		protected override Type DetermineSerializationType(){
			if (this.operatingMode == APIResourceMode.Query) {
				return typeof(BatchResultSet);
			} else {
				return typeof(BatchRecord);
			}
		}
	}
}

