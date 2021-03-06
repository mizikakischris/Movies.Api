﻿using Movie.Types.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Movie.Types.Dtos
{
    [DataContract]
    public class ActorDto
    {
        [DataMember(Name="Id")]
        public int Id { get; set; }

        [Required]
        [DataMember]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [DataMember(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Date Of Birth")]
        [DataMember(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }

        [DataMember(Name = "Picture")]
        public byte[] Picture { get; set; }

        //MoviesPlayedByActor
        [DataMember(Name = "Movies")]
        public List<MoviesByActorDto> Movies { get; set; }
    }
}
