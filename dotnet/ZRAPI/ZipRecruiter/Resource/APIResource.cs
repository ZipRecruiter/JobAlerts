/* APIResources.cs
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

namespace ZipRecruiter.Resource
{
	public enum APIResourceMode { None, Query, Submit, Retrieve, Deactivate }; 

	/// <summary>
	/// Base class for API resources. Not to be instantiated on its own.
	/// </summary>
	public class APIResource : ICloneable {

		#region Protected instance variables

		protected JobAlertsAPI 				handle;
		protected APIResourceMode 	operatingMode	= APIResourceMode.None;
		protected String 			subscriberID;
		protected String 			jobSearchID;
		protected String 			batchID;
		protected String 			endpoint;

		#endregion

		/// <summary>
		/// Gets or sets request parameters.
		/// Setting the property Works in an append/replace mode and existing keys will remain.
		/// To reset the Parameters call ClearParameters();
		/// </summary>
		/// <value>Request parameters.</value>
		public Dictionary<String,String> Parameters {
			get {
				return _parameters;
			}
			protected set {
				foreach (KeyValuePair<String,String> v in value) {
					_parameters.Add (v.Key, v.Value);
				}
			}
		}

		private Dictionary<String,String> 	_parameters 	= new Dictionary<string, string>();

		#region Constructors

		public APIResource(JobAlertsAPI handle){
			this.handle = handle;
		}

		#endregion

		#region Public API Accessors

		/// <summary>
		/// Attempts to retrieve a record using the given parameters using a GET.
		/// Arguments are optional and context sensitive. An exception will be thrown if an invalid parameter is given.
		/// </summary>
		/// <param name="subscriberID">Subscriber ID.</param>
		/// <param name="jobSearchID">Job Search ID.</param>
		/// <param name="batchID">Batch ID.</param>
		/// <returns>If found, the correct Record for the specified type.</returns>
		public virtual dynamic Retrieve (String subscriberID = "",
		                            String jobSearchID = "",
		                            String batchID = ""){
			this.subscriberID = subscriberID.Equals("") ? this.subscriberID : subscriberID;
			this.jobSearchID = jobSearchID.Equals("") ? this.jobSearchID : jobSearchID;
			this.batchID = batchID.Equals("") ? this.batchID : batchID;
			this.operatingMode = APIResourceMode.Retrieve;
			return this;
		}

		/// <summary>
		/// Attempts to retrieve a record using the given parameters using a POST.
		/// Arguments except the "parameters" Dictionary are optional and context sensitive. An exception will be thrown if an invalid parameter is given.
		/// </summary>
		/// <param name="parameters">Parameters.</param>
		/// <param name="subscriberID">Subscriber I.</param>
		/// <param name="jobSearchID">Job search I.</param>
		/// <param name="batchID">Batch I.</param>
		/// <returns>If successful, the correct Result record for the specified type.</returns>
		public virtual dynamic Submit (Dictionary<String,String> parameters,
		             String subscriberID = "",
		             String jobSearchID = "",
		             String batchID = ""){
			this.Parameters = parameters;
			this.subscriberID = subscriberID.Equals("") ? this.subscriberID : subscriberID;
			this.jobSearchID = jobSearchID.Equals("") ? this.jobSearchID : jobSearchID;
			this.batchID = batchID.Equals("") ? this.batchID : batchID;
			this.operatingMode = APIResourceMode.Submit;
			return this;
		}

		/// <summary>
		/// Attempts to search for records matching the given parameters.
		/// Parameters are optional and context sensitive. An exception will be thrown if an invalid parameter is given.
		/// This methods lacks a "parameters" dictionary argument and as a result will grab all records for the specified endpoint.
		/// </summary>
		/// <param name="subscriberID">Subscriber I.</param>
		/// <param name="jobSearchID">Job search I.</param>
		/// <param name="batchID">Batch I.</param>
		/// <returns>The correct ResultSet record for the specified type.</returns>
		public virtual dynamic Query (String subscriberID = "",
		                              String jobSearchID = "",
		                              String batchID = ""){
			return this.Query (new Dictionary<String,String>(),
			                  subscriberID: subscriberID,
			                  jobSearchID: jobSearchID,
			                  batchID: batchID);
		}

		/// <summary>
		/// Attempts to search for records matching the given parameters.
		/// Parameters are optional and context sensitive. An exception will be thrown if an invalid parameter is given.
		/// This methods has a "parameters" dictionary argument and the results will be augmented by its contents.
		/// </summary>
		/// <param name="parameters">Parameters.</param>
		/// <param name="subscriberID">Subscriber I.</param>
		/// <param name="jobSearchID">Job search I.</param>
		/// <param name="batchID">Batch I.</param>
		/// <returns>The correct ResultSet record for the specified type.</returns>
		public virtual dynamic Query (Dictionary<String,String> parameters,
		             String subscriberID = "",
		             String jobSearchID = "",
		             String batchID = ""){
			this.Parameters = parameters;
			this.subscriberID = subscriberID.Equals("") ? this.subscriberID : subscriberID;
			this.jobSearchID = jobSearchID.Equals("") ? this.jobSearchID : jobSearchID;
			this.batchID = batchID.Equals("") ? this.batchID : batchID;
			this.operatingMode = APIResourceMode.Query;
			return this;
		}

		/// <summary>
		/// Attempts to deactivate a record using the given parameters.
		/// Parameters are optional and context sensitive. An exception will be thrown if an invalid parameter is given.
		/// </summary>
		/// <param name="subscriberID">Subscriber I.</param>
		/// <param name="jobSearchID">Job search I.</param>
		/// <param name="batchID">Batch I.</param>
		/// <returns>If found, the correct Result record for the specified type.</returns>
		public virtual dynamic Deactivate (String subscriberID = "",
		                              String jobSearchID = "",
		                              String batchID = ""){
			this.subscriberID = subscriberID.Equals("") ? this.subscriberID : subscriberID;
			this.jobSearchID = jobSearchID.Equals("") ? this.jobSearchID : jobSearchID;
			this.batchID = batchID.Equals("") ? this.batchID : batchID;
			this.operatingMode = APIResourceMode.Deactivate;
			return this;
		}

		#endregion

		/// <summary>
		/// Clears the Parameters dictionary.
		/// </summary>
		/// <returns>The ApiResource subclass instance the method was invoked upon. Allows chaining.</returns>
		public APIResource ClearParameters(){
			_parameters.Clear();
			return this;
		}

		/// <summary>
		/// Adds to the parameters dictionary.
		/// </summary>
		/// <returns>The ApiResource subclass instance the method was invoked upon. Allows chaining.</returns>
		/// <param name="parameters">Request parameters.</param>
		public APIResource AddParameters(Dictionary<String,String> parameters){
			this.Parameters = parameters;
			return this;
		}

		/// <summary>
		/// Adds a single key/value pair to the parameter dictionary.
		/// </summary>
		/// <returns>The ApiResource subclass instance the method was invoked upon. Allows chaining.</returns>
		/// <param name="key">Parameter Key.</param>
		/// <param name="value">Parameter Value.</param>
		public APIResource AddParameter(String key,String value){
			if (!this._parameters.ContainsKey (key)) {
				this._parameters.Add (key, value);
			}
			return this;
		}

		/// <summary>
		/// Removes the a single parameter from the parameter dictionary.
		/// </summary>
		/// <returns>The ApiResource subclass instance the method was invoked upon. Allows chaining.</returns>
		/// <param name="key">Key.</param>
		public APIResource RemoveParameter(String key){
			if (this._parameters.ContainsKey (key)) {
				this._parameters.Remove (key);
			}
			return this;
		}

		/// <summary>
		/// Performs the API call which has been constructed.
		/// </summary>
		/// <returns>Subclasses return the propper result class type.
		/// This method should not be invoked on the base class as it returns an incomplete WebRequest.</returns>
		public virtual dynamic Call()
		{
			this.AddParameter("expand_results", "1");
			System.Net.WebRequest request = System.Net.WebRequest.Create(this.BuildRequestUrl());

			//Throw an exception if no action has been invoked previously
			if (this.operatingMode == APIResourceMode.None){
				throw new MissingResourceActionException ();
			}

			if (this.operatingMode == APIResourceMode.Submit) {
				request.Method = "POST";
			} else if (this.operatingMode == APIResourceMode.Query || this.operatingMode == APIResourceMode.Retrieve) {
				request.Method = "GET";
			} else if (this.operatingMode == APIResourceMode.Deactivate) {
				request.Method = "DELETE";
			}

			return request;
		}

		/// <summary>
		/// Deep copy this instance.
		/// </summary>
		public virtual Object Clone(){
			APIResource clone = (APIResource)this.MemberwiseClone ();
			clone._parameters = new Dictionary<String,String> (this._parameters);

			return clone;
		}

		#region Protected Methods

		/// <summary>
		/// Builds the request URL.
		/// </summary>
		/// <returns>The request URL.</returns>
		protected virtual String BuildRequestUrl () { 
			return JobAlertsAPI.Endpoints[this.endpoint];
		}

		/// <summary>
		/// Uses the class's instance variables to determine the propper Type
		/// to be given to the XMLDeseralizer when invoked by the Call method.
		/// </summary>
		/// <returns>The serialization type.</returns>
		protected virtual Type DetermineSerializationType(){
			return typeof(Object);
		}

		#endregion
	}
}

