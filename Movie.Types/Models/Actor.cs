﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Movie.Types.Models
{
   public class Actor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Date Of Birth")]
        public decimal DateOfBirth { get; set; }

        public byte[] Picture { get; set; }


        //An Actor can appear multiple times in the Movies table
        List<MovieModel> Movies { get; set; }

        //Reference Navigation Property
        public Hero Hero { get; set; }
    }
}
