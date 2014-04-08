/* Examples.cs
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
using ZipRecruiter;
using System.Collections.Generic;

namespace ZRAPI
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			/*
			 * Quick run-through of possible gotchas:
			 * Wrap calls in Try/Catch blocks. This API throws its own exceptions and makes
			 * no attempts to catch WebRequest and XMLSerializer exceptions
			 * 
			 * I am yet to test all possible combinations of Action chaining, i.e.
			 * Query(...).Submit(...) will likely cause problems.
			 * 
			 * Chaining the same Action...
			 * Query(...).Query(...) should be just fine however.
			 * 
			 * Subscriber querying is very slow at times. It's best to store the ID of a Subscriber once you
			 * have it if you plan on needing it again (or store the ID given in the response of a Submit action).
			 * 
			 * While all actions are available to the Resource objects, the only valid ones are the analogs
			 * specified in the ZipRecruiter API Documentation.
			 */ 

			//Set an API Key globally. Your API Key is available in your ZipAlerts control panel
			ZipRecruiter.JobAlertsAPI.GlobalAPIKey = "";
			//Setting to true means we'll use the predefined Dev API servers and output
			ZipRecruiter.JobAlertsAPI.DevelopmentAPIMode = true;
			//Log received XML to the Console (Console.WriteLine)
			ZipRecruiter.JobAlertsAPI.WriteXMLToConsole = true;

			var instance = JobAlertsAPI.SharedInstance;

			Console.WriteLine ("The SharedInstance API Key is : " + instance.APIKey);

			var spotInstance = new JobAlertsAPI("thisisnotakey");

			Console.WriteLine ("The standard instance's API Key is : " + spotInstance.APIKey);

			spotInstance = new JobAlertsAPI("teigheer4esh9too3vah3oor9aish9ru");

			//Nothing wrong with instantiating manually
			ZipRecruiter.Resource.BatchResource batchResource = new ZipRecruiter.Resource.BatchResource(spotInstance);

			//Same thing but less verbose
			ZipRecruiter.Resource.BatchResource batchResource2 = spotInstance.Resource("batch");

			//Get every batch record
			ZipRecruiter.Schema.BatchResultSet batches = batchResource.Query().Call();

			if (batches.Results.Length > 0) {
				//SearchResult classes are lightweight objects which can be used to pull Record class objects
				ZipRecruiter.Schema.BatchRecord batchSearchResult = batches.Results[0];
				DateTime? create_time = batchSearchResult.CreateTime;
			}

			//Post a subscriber file

			String fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".csv";
			String csv = "\"user1@domain1.com\",\"Legal Assistant\",\"Houston, TX\",\"2012-05-03 09:45:23\",\n\"user2@domain2.com\",\"CEO\",\"10023\",\"2012-05-01 17:01:05\",\n\"user3@domain3.com\",\"Snakeoil Salesman\",,\"2012-05-02 12:00:01\",\"194.133.209.43\"";
			System.IO.File.AppendAllText (fileName, csv);

			//var result = batchResource2.Submit(new Dictionary<String,String>(){{"type", "subscribe"},{"content",fileName}}).Call();

			/*
			 * Subscriber Calls 
			 */

			//Static Typing
			ZipRecruiter.Schema.SubscriberRecord subscriber = null;
			
			try {
			  subscriber = instance.Resource ("subscriber").Retrieve (subscriberID:"f2c3ce7e").Call ();
			  Console.WriteLine ("Subscriber email for f2c3ce7e: " + subscriber.EmailMD5);
			} catch(Exception ex) {

			}
			//Query for all subscribers belonging to a particular brand
			ZipRecruiter.Schema.SubscriberResultSet allStarkJobsSubscribers = instance.Resource("subscriber").Query(new Dictionary<String,String>(){ { "brand","Stark Jobs" }, }).Call();

			//Dynamic with a static cast. Gains you Intellisense
			var allSubscribers = (ZipRecruiter.Resource.SubscriberResource)instance.Resource ("subscriber").Query ();

			//Dynamic with runtime cast. Gains you Intellisense and you can test the cast
			var subscribers = allSubscribers.Call () as ZipRecruiter.Schema.SubscriberResultSet;

			if (subscriber != null) {
				Console.WriteLine ("Number found: {0:N}", subscribers.TotalCount);
			} else {
				Console.WriteLine ("There was a problem");
			}

			string sub2Id = subscribers.Results[0].ID;

			var sub2IdJobsearch = instance.Resource("job_search").Query(new Dictionary<string, string>(){},subscriberID:sub2Id).Call();

			//Dynamic typing is fine
			var subscriber2 = instance.Resource ("subscriber").Retrieve(subscriberID:sub2Id).Call ();
			//Console.WriteLine ("Subscriber email for " + sub2Id + ": " + subscriber.EmailMD5);

			//Return types are dynamic in the first place, so we can use .Net 4.0 dynamic typing
			dynamic subscriber3 = subscribers.Results [1];
			Console.WriteLine ("subscriber3's type is " + subscriber3.GetType().ToString());
			Console.WriteLine ("Subscriber email for " + subscriber3.ID + ": " + subscriber3.EmailMD5);
			Console.WriteLine ("Subscriber " + subscriber3.ID + " is active? : " + (String.IsNullOrEmpty(subscriber3.DeactivationReason) ? "Active" : subscriber3.DeactivationReason));

			//Uncomment if you'd like to test, note this example will deactivate whatever the first result was
			Console.WriteLine ("Deactivating: " + subscriber3.ID);
			var deactivatedContact = instance.Resource ("subscriber").Deactivate (subscriberID:subscriber3.ID).Call();

			//Reuse a previous Query Resource
			ZipRecruiter.Schema.SubscriberResultSet anotherWay = allSubscribers.AddParameter ("email_md5", subscriber3.EmailMD5).Call();
			Console.WriteLine("Found: " + anotherWay.TotalCount);

			//We can make the same call again to get a fresh copy.
			subscriber3 = subscribers.Results [1];
			Console.WriteLine ("Subscriber " + subscriber3.ID + " is active? : " + (String.IsNullOrEmpty(subscriber3.DeactivationReason) ? "Active" : subscriber3.DeactivationReason));

			/*
			 * Job Search Calls
			 */ 

			ZipRecruiter.Schema.JobSearchResultSet jobsearch = null;

			//The Resource classes throw errors and do not attempt to catch HTTP errors from the
			//WebRequest class so you should wrap calls in try/catch blocks if they may fail.
			//Note, an empty resultset will not cause an exception.
			try {
				jobsearch = instance.Resource("job_search").Query(subscriberID:"f2c3ce7e").Call();
			} catch(Exception ex) {
				Console.WriteLine ("An exception occured: " + ex.Message);
			}

			ZipRecruiter.Schema.JobSearchRecord jobsearch2 = instance.Resource("job_search").Retrieve(subscriberID:"f2c3ce7e",jobSearchID:"b6f2266a").Call();

			var jobsearch3 = instance.Resource("job_search").Query(new Dictionary<string, string>(),subscriberID:"f2c3ce7e").Call();

			var jobsearch4 = instance.Resource("job_search").Submit(new Dictionary<string, string>(){
				{"search","Legal Assistant"},
				{"ip_address","108.0.247.154"},
			},subscriberID:"f2c3ce7e").Call();

			var jobsearch5 = instance.Resource("job_search").Query(new Dictionary<string, string>(){
				{"location","Dallas, TX"},
			},subscriberID:"f2c3ce7e").Call();

			var searchjobsearch = jobsearch5 as ZipRecruiter.Schema.JobSearchResultSet;

			if (searchjobsearch != null){
				String deactivationTarget = jobsearch5.Results[0].ID;
				var deactivated = instance.Resource("job_search").Deactivate(subscriberID:"f2c3ce7e",jobSearchID:deactivationTarget).Call();
				var obj = jobsearch5.Results[0];
			}

			if (searchjobsearch != null) {
				var d = jobsearch.Results;
				foreach (var dt in d) {
					Console.WriteLine (dt.ID);
				}
			}			

			Console.WriteLine ("Hello World!");
		}
	}
}
