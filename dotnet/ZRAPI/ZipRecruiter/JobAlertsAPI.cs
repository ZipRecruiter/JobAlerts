/* JobAlertsAPI.cs
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
using System.Net;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using ZipRecruiter.Schema;
using ZipRecruiter.Resource;

namespace ZipRecruiter
{
	#region Exceptions

	/// <summary>
	/// Invalid endpoint exception.
	/// Thrown when an unrecognized API endpoint is specified to the JobAlertsAPI class
	/// </summary>
	public class InvalidEndpointException : Exception
	{
		public String Endpoint { get; private set; }
		public InvalidEndpointException(String endpoint) : base("The given ZipRecruiter API Endpoint \"" + endpoint + "\" is invalid") 
		{
			Endpoint = endpoint;
		}
	}

	/// <summary>
	/// Missing API arguments exception.
	/// Thrown when a Action's required argument is missing.
	/// </summary>
	public class MissingAPIArgumentsException : Exception
	{
		public ArrayList Arguments { get; private set; }
		public String Endpoint { get; private set; }
		public MissingAPIArgumentsException(String endpoint, ArrayList arguments) 
			: base("One or more required fields were missing when attempting to make a \"" + endpoint + "\" API call") 
		{
			Endpoint = endpoint;
			Arguments = arguments;
		}
	}

	/// <summary>
	/// Extraneous API arguments exception.
	/// Thrown when an argument is given to an Action erroneously 
	/// </summary>
	public class ExtraneousAPIArgumentsException : Exception
	{
		public ArrayList Arguments { get; private set; }
		public String Endpoint { get; private set; }
		public ExtraneousAPIArgumentsException(String endpoint, ArrayList arguments) 
			: base("One or more specified fields should not be given when making a \"" + endpoint + "\" API call") 
		{
			Endpoint = endpoint;
			Arguments = arguments;
		}
	}

	/// <summary>
	/// Missing ZipRecruiter API key exception.
	/// Thrown by a JobAlertAPI object constructor when there is no GlobalAPIKey set and no API key was given to the instance being created.
	/// </summary>
	public class MissingZipRecruiterAPIKeyException : Exception
	{
		public MissingZipRecruiterAPIKeyException() : base("No API Key was specified") 
		{
		}
	}

	/// <summary>
	/// Missing resource action exception.
	/// Thrown when APIResource.Call() is invoked without an Action method being invoked first.
	/// </summary>
	public class MissingResourceActionException : Exception
	{
		public MissingResourceActionException() : 
			base("Action was specified before Call was invoked. Please call an appropriate Action first (i.e. Retrieve, Query, Submit, or Deactivate)") 
		{
		}
	}

	#endregion

	/// <summary>
	/// API Object used by Resource classes which API Call specific data and preferences.
	/// Includes a simple interface to instantiate Resource objects, useful for creating chained calls.
	/// </summary>
	public class JobAlertsAPI
	{
		#region API Endpoints

		/// <remarks>
		/// Production API endpoints
		/// </remarks>
		private static readonly Dictionary<String,String> ProductionEndpoints = new Dictionary<String, String>(){
			{"batch","https://api.ziprecruiter.com/job-alerts/v2/batch"},
			{"subscriber","https://api.ziprecruiter.com/job-alerts/v2/subscriber"},
			{"job_search","https://api.ziprecruiter.com/job-alerts/v2/subscriber/SUBSCRIBER_ID/searches"},
		};

		/// <remarks>
		/// Development API endpoints. Currenlty points to Max's sandbox
		/// </remarks>
		private static readonly Dictionary<String,String> DevelopmentEndpoints = new Dictionary<String, String>(){
			{"batch","http://apidev.ziprecruiter.com:4014/job-alerts/v2/batch"},
			{"subscriber","http://apidev.ziprecruiter.com:4014/job-alerts/v2/subscriber"},
			{"job_search","http://apidev.ziprecruiter.com:4014/job-alerts/v2/subscriber/SUBSCRIBER_ID/searches"},
		};

		/// <remarks>
		/// Used to determin whether the Development API Endpoints will be used or not.
		/// </remarks>
		public static bool DevelopmentAPIMode { get; set; }

		/// <remarks>
		/// Used to determin whether the received XML will be output to the console.
		/// </remarks>
		public static bool WriteXMLToConsole { get; set; }

		/// <remarks>
		/// The dictionary of active endpoints
		/// </remarks>
		public static Dictionary<String,String> Endpoints {
			get {
				return DevelopmentAPIMode ? DevelopmentEndpoints : ProductionEndpoints;
			}
		}

		#endregion

		/// <remarks>
		/// Get/Sets a default API Key to be used by the API constructor when a key isn't specified
		/// </remarks>
		public static String GlobalAPIKey { get; set; }

		/// <summary>
		/// Gets a shared instance. Requires that GlobalAPIKey be set before being called or an exception will be thrown.
		/// </summary>
		/// <value>The shared instance.</value>
		public static JobAlertsAPI SharedInstance {
			get {
				if (_instance == null) {
					lock (_lock) {
						if (_instance == null) {
							_instance = new JobAlertsAPI ();
						}
					}
				}
				return _instance;
			}
		}
		private static JobAlertsAPI _instance;
		private static object _lock = new Object();

		/// <summary>
		/// Gets the API key.
		/// </summary>
		/// <value>The API key.</value>
		public String APIKey { 
			get; 
			set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ZipRecruiter.API"/> class.
		/// Initialized the internal APIKey using the GlobalAPIKey.
		/// </summary>
		public JobAlertsAPI() : this(GlobalAPIKey){ }

		/// <summary>
		/// Initializes a new instance of the <see cref="ZipRecruiter.API"/> class.
		/// </summary>
		/// <param name="apiKey">The desired Private ZipRecruiter API key</param>
		public JobAlertsAPI (String apiKey)
		{
			if (String.IsNullOrEmpty (apiKey)) {
				throw new MissingZipRecruiterAPIKeyException();
			}
			APIKey = apiKey;
		}

		/// <summary>
		/// Augments the given WebRequest object to match the needs of the API and returns a deserialized native type.
		/// </summary>
		/// <returns>The deserialized Record, ResultSet, or Result object.</returns>
		/// <param name="request">A predefined request object containing the proper URL and Method for the request.</param>
		/// <param name="serializerType">The target deserialization type.</param>
		/// <param name="body">Optional - The desired request body to be included in POST requests.</param>
		internal dynamic MakeRequest(WebRequest request, Type serializerType, String body = ""){

			request.Headers ["Authorization"] = "Basic " + Convert.ToBase64String (Encoding.UTF8.GetBytes(this.APIKey + ':'));
			request.ContentType = "text/xml";

			if (request.Method.Equals ("POST")) {
				request.ContentType="application/x-www-form-urlencoded";
				byte [] bytes = System.Text.Encoding.ASCII.GetBytes(body);
				request.ContentLength = bytes.Length;
				System.IO.Stream os = request.GetRequestStream ();
				os.Write (bytes, 0, bytes.Length); //Push it out there
				os.Close ();
			}

			return DeserializeResults(request.GetResponse().GetResponseStream(),serializerType);
		}

		/// <summary>
		/// Augments the given WebRequest object to match the needs of the API and returns a deserialized native type.
		/// </summary>
		/// <returns>The deserialized Record, ResultSet, or Result object.</returns>
		/// <param name="request">A predefined request object containing the proper URL and Method for the request.</param>
		/// <param name="serializerType">The target deserialization type.</param>
		/// <param name="body">Optional - The desired request body to be included in POST requests.</param>
		internal dynamic MakeRequest(WebRequest request, Type serializerType, Byte[] body){

			request.Headers ["Authorization"] = "Basic " + Convert.ToBase64String (Encoding.UTF8.GetBytes(this.APIKey + ':'));

			if (request.Method.Equals ("POST")) {
				request.ContentLength = body.Length;
				System.IO.Stream os = request.GetRequestStream ();
				os.Write(body, 0, body.Length); //Push it out there
				os.Close();
			}

			return DeserializeResults(request.GetResponse().GetResponseStream(),serializerType);
		}

		/// <summary>
		/// Deserializes the results of the API call into a usable native type.
		/// </summary>
		/// <returns>The deserialized Record, ResultSet, or Result object</returns>
		/// <param name="responseData">Raw response data from the server</param>
		/// <param name="serializerType">The target deserialization type.</param>
		protected dynamic DeserializeResults(Stream responseData, Type serializerType){
			String sDS = new StreamReader (responseData).ReadToEnd ();
			sDS = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" + sDS;
			StreamReader serializedReader = new StreamReader (new MemoryStream(Encoding.UTF8.GetBytes(sDS)));
			if (WriteXMLToConsole) {
				Console.WriteLine (sDS);
			}

			XmlSerializer serializer = new XmlSerializer(serializerType);
			return serializer.Deserialize(serializedReader);
		}

		/// <summary>
		/// Uses the given resource endpoint to return the propper ApiResource class to invoke calls on.
		/// </summary>
		/// <param name="endpoint">The desired API Resource endpoint</param>
		public dynamic Resource(String endpoint){
			APIResource resource = null;

			switch (endpoint) {
			case "subscriber":
				resource = new SubscriberResource(this);
				break;
			case "job_search":
				resource = new JobSearchResource(this);
				break;
			case "batch":
				resource = new BatchResource(this);
				break;
			default:
				throw new InvalidEndpointException(endpoint);
			}

			return resource;
		}
	}
}

