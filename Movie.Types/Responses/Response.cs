﻿using Movie.Api.Exceptions;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Movie.Types.Responses
{
    [DataContract]
   public class Response<T> where T: class
    {
        [DataMember]
        public string Version { get { return "1.1"; } }

        [DataMember(Name = "payload")]
        public Payload<T> Payload { get; set; }

        [DataMember(Name ="exception")]
        public ErrorDetails Exception { get; set; }

    }

   
    public class Payload<T> where T: class
    {
      public  T movie;
      public List<T> movies;
        
    }
}