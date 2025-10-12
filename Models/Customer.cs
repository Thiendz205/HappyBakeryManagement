﻿namespace HappyBakeryManagement.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber {  get; set; }
        public string Address { get; set; }
        public string CitizenID { get; set; }
        public bool Gender { get; set; }
        public DateTime DOB { get; set; }
    }
}
