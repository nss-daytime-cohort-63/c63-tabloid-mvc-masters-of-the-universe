﻿using System.Collections.Generic;
namespace TabloidMVC.Models.ViewModels
{
    public class PostDetailsViewModel
    {
        public Post Post { get; set; }
        public Subscription ActiveSubscription { get; set; }
    }
}