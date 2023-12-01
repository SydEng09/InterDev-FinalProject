using ServiceSystem.DAL;
using ServiceSystem.Entities;
using ServiceSystem.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceSystem.BLL
{
    public class ServicingServices
    {
        private readonly eBikeServiceContext _context;

        internal ServicingServices(eBikeServiceContext context)
        {
            _context = context;
        }
        List<Exception> errorList = new List<System.Exception>();

        //get customer
        public async Task<List<CustomerView>> FetchCustomer(string lastName)
        {
            bool customerExists = false;

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentNullException("Last name of a customer was not submited");
            }

            customerExists = _context.Customers
            .Where(x => x.LastName.Contains(lastName))
            .Select(x => x.CustomerID)
            .Any();

            if (!customerExists)
            {
                throw new ArgumentNullException ($"Customer with LastName ''{lastName}'' does not exist in the database");
            }

            return _context.Customers.Where(x => x.LastName.Contains(lastName)).Select(x => new CustomerView
            {
                CustomerID = x.CustomerID,
                Name = x.FirstName + " " + x.LastName,
                Address = x.Address,
                Phone = x.ContactPhone

            }).ToList();

        }

        // get customer vehicles 
            public async Task<List<CustomerVehicleView>> FetchCustomerVehicle(int customerID)
            {
                bool hasVehicle = false;

                hasVehicle = _context.CustomerVehicles
                .Where(x => x.CustomerID == customerID)
                .Select(x => x.VehicleIdentification)
                .Any();

                if (!hasVehicle)
                {
                    throw new ArgumentNullException($"Selected customer with CustomerID ''{customerID}'' doest not have a Vehicle registered");
                }


                return _context.CustomerVehicles.Where(x => x.CustomerID == customerID).Select(x => new CustomerVehicleView
                {

                    VehicleIdentification = x.VehicleIdentification,
                    MakeModel = x.Make + " " + x.Model

                }).ToList();


            }
        // get standerd service 
        public List<CategoryView> FetchStandrdService()
        {

            return _context.StandardJobs.Select(x => new CategoryView
            {

                CategoryID = x.StandardJobID,
                Description = x.Description

            }).ToList();
        }


        //adding 
        public JobDetailView AddService(string service, decimal hours, string comment)
        {
            

            if (string.IsNullOrWhiteSpace(service))
            {
                throw new ArgumentNullException("Please enter the name of service");
            }
            if (hours <= 0.00M)
            {
                throw new ArgumentException("Hours must be filled out and cant be a negative or equal to zero");
            }
            JobDetailView jobDetailView = new JobDetailView();
            string serviceDesc = service;
            decimal serviceHrs = hours;
            string serviceComment = comment;

            jobDetailView.Description = serviceDesc;
            jobDetailView.JobHours = serviceHrs;
            jobDetailView.Comment = serviceComment;

            return jobDetailView;

        }
        //Quick add
        public JobDetailView AddServiceQuick(int serviceId) 
        {
            JobDetailView jobDetailView = new JobDetailView();
            int serviceid = serviceId;
            string serviceDec = _context.StandardJobs.Where(x => x.StandardJobID == serviceId).Select(x => x.Description).FirstOrDefault();
            decimal serviceHrs = _context.StandardJobs.Where(x => x.StandardJobID == serviceId).Select(x => x.StandardHours).FirstOrDefault();
            string serviceComment = null;

            jobDetailView.Description = serviceDec;
            jobDetailView.JobHours = serviceHrs;
            jobDetailView.Comment = serviceComment;

            return jobDetailView;
        }

        // for coupon
        public async Task<int> GetCoupon(string couponIDValue)
        {
            int discountAmount = 0;

            if (String.IsNullOrWhiteSpace(couponIDValue))
            {
                throw new ArgumentNullException("Coupon code was not submitted");
            }

            DateTime couponStart = _context.Coupons
                                        .Where(x => x.CouponIDValue == couponIDValue)
                                        .Select(x => x.StartDate)
                                        .FirstOrDefault();
            DateTime couponEnd = _context.Coupons
                                        .Where(x => x.CouponIDValue == couponIDValue)
                                        .Select(x => x.EndDate)
                                        .FirstOrDefault();

            if (couponStart > DateTime.Now || couponEnd < DateTime.Now)
            {
                throw new Exception("Entered coupon code is not available at the moment");
            }

            bool isService = false;

            isService = _context.Coupons.Where(x => x.CouponIDValue == couponIDValue && x.SalesOrService == 2)
                                .Select(x => x.CouponDiscount)
                                .Any();

            if (!isService)
            {
                throw new ArgumentNullException($"Entered coupon code is not valid for this type of service");
            }

            return discountAmount = _context.Coupons.Where(x => x.CouponIDValue == couponIDValue).Select(x => x.CouponDiscount).First();

        }
        //for registering service
        public async Task<int> RegisterJob(List<JobDetailView> jobDetails, string description, decimal jobHours, string couponIdValue, int employeeId, string vehicleIdentificationNumber, decimal shopeRate, string comment, int serviceId)
        {

            if (jobDetails.Count() == 0)
            {
                throw new ArgumentNullException("Job details must be supplied in order to register");
            }
            else
            {

                int couponId = _context.Coupons.Where(x => x.CouponIDValue == couponIdValue).Select(x => x.CouponID).FirstOrDefault();
                Job currentJobView = new Job();
                currentJobView.JobDateIn = DateTime.Now;
                currentJobView.VehicleIdentification = vehicleIdentificationNumber;
                currentJobView.EmployeeID = employeeId;
                currentJobView.ShopRate = shopeRate;
                _context.Jobs.Add(currentJobView);
                foreach (var jd in jobDetails)
                {

                    JobDetail parms = new JobDetail();
                    if (couponId <= 0)
                    {
                        parms.CouponID = null;
                    }
                    else
                    {
                        parms.CouponID = couponId;
                    }
                    if (serviceId > 0)
                    {
                        parms.Description = _context.StandardJobs.Where(x => x.StandardJobID == serviceId).Select(x => x.Description).FirstOrDefault();
                        parms.JobHours = _context.StandardJobs.Where(x => x.StandardJobID == serviceId).Select(x => x.StandardHours).FirstOrDefault();
                        parms.Comments = null;
                    }
                    else
                    {
                        parms.Description = description;
                        parms.JobHours = jobHours;
                        parms.Comments = comment;
                    }





                    currentJobView.JobDetails.Add(parms);
                }


                _context.SaveChanges();
                int JobId = _context.Jobs.Select(x => x.JobID).Max();
                return JobId;
            }
        }
    }
}
