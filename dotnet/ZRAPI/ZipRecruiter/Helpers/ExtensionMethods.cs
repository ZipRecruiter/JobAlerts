/* ExtensionMethods.cs
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

namespace ZipRecruiter
{
	internal static class ExtensionMethods
	{
		public static String ToPostBody(this Dictionary<String,String> value){
			if (value.Count == 0) {
				return "";
			}

			StringBuilder sb = new StringBuilder ();
			int keycount = 1;
			foreach (KeyValuePair<String,String> v in value) {
				if (v.Value != null){
					sb.Append (Uri.EscapeDataString(v.Key));
					sb.Append ("=");
					sb.Append (Uri.EscapeDataString(v.Value));
					if (keycount++ != value.Count) {
						sb.Append ("&");
					}
				}
			}
			return sb.ToString();
		}

		public static String ToQueryString(this Dictionary<String,String> value){
			if (value.Count == 0) {
				return "";
			}

			StringBuilder sb = new StringBuilder ();
			sb.Append ("?");
			int keycount = 1;
			foreach (KeyValuePair<String,String> v in value) {
				if (v.Value != null) {
					sb.Append (Uri.EscapeUriString (v.Key));
					sb.Append ("=");
					sb.Append (Uri.EscapeUriString (v.Value));
					if (keycount++ != value.Count) {
						sb.Append ("&");
					}
				}
			}
			return sb.ToString();
		}
	}
}

