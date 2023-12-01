
using SalesSystem.DAL;
using SalesSystem.Entities;
using SalesSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesSystem.BLL
{
    public class SaleServices
    {
        private readonly eBikeContext _context;

        internal SaleServices(eBikeContext context)
        {
            _context = context;
        }

        List<Exception> errorList = new List<System.Exception>();
        bool partExistsinDatabase = false;
       

        //Sale sales = new Sale();
        List<SaleDetailView> salesdetailCart = new List<SaleDetailView>();

        public SaleDetailView AddPart(int partId, int Quantity)
        {
            SaleDetailView saledetail = new SaleDetailView();
            int selectedPartId = partId;

            Part selectedPart = _context.Parts.Where(x => x.PartID == partId).Select(x => x).FirstOrDefault();
            if (selectedPart == null)
            {
                throw new ArgumentNullException("Select a part to add first");
            }
            if (Quantity <= 0)
            {
                throw new ArgumentNullException("Must enter a positive Quantity to add part into cart");
            }


            if (selectedPart.QuantityOnHand < Quantity)
            {
                errorList.Add(new Exception("Your added quanity can't be more then quanitity on hand"));
            }
            else
            {


                partExistsinDatabase = _context.Parts.Any(x => x.PartID == selectedPartId);

                if (partExistsinDatabase != true)
                {
                    errorList.Add(new Exception("This part no longer available in the database, Please try again"));
                }
                else
                {
                   
                   
                    

                        string productname = selectedPart.Description;
                        int Qoh = selectedPart.QuantityOnHand;
                        decimal price = selectedPart.SellingPrice;

                        
                        saledetail.PartId = selectedPartId;
                        saledetail.PartName = productname;
                        saledetail.PartPrice = price;
                        saledetail.Qty = Quantity;
                        saledetail.QoH = Qoh;
                        saledetail.total = (saledetail.Qty * saledetail.PartPrice);







                }
               

              
                
            }
            if (errorList.Count() > 0)
            {

                throw new AggregateException("Unable to add new part.  Check concerns", errorList);
            }
            else
            {
                return saledetail;
            }
            

        }
      
        public Task< List<CategorySelectionView>> GetCategories()
        {

            return Task.FromResult(_context.Categories
            .Select(x => new CategorySelectionView
            {
                CategoryId = x.CategoryID,
                CategoryDescription = x.Description

            }).ToList());

        }
        public async Task<List<ProductSelectionView>> GetProducts(int CategoryId)
        {
            if (CategoryId == 0)
            {
                throw new ArgumentNullException("You need to select a category first");
            }
            else
            {
                return _context.Parts.Where(x => x.CategoryID == CategoryId)
                .Select(x => new ProductSelectionView
                {
                    PartId = x.PartID,
                    Price = x.SellingPrice,
                    ProductName = x.Description,
                    QoH = x.QuantityOnHand

                }).ToList();
            }
        }
      
        public async Task<int> GenerateInvoice(List<SaleDetailView> saledetailCart, string paymentType, int employeeId, string couponCode, decimal subtotal, decimal tax)
        {
            if (string.IsNullOrWhiteSpace(paymentType))
            {
                throw new ArgumentNullException("Please enter payment type");

            }
            else if (saledetailCart.Count() == 0)
            {
                throw new ArgumentNullException("You need to have items in your cart in order to purchase");
            }
            else
            {
                int couponId;
                couponId = _context.Coupons.Where(x => x.CouponIDValue == couponCode).Select(x => x.CouponID).FirstOrDefault();
                //int discountPercentage = _context.Coupons.Where(x => x.CouponID == couponId).Select(x => x.CouponDiscount).FirstOrDefault();
               // Console.WriteLine(discountPercentage);
                Sale currentSaleView = new Sale();

                if (couponId > 0)
                {
                    currentSaleView.CouponID = couponId;
                }
                else
                {
                    currentSaleView.CouponID = null;
                }

                
                currentSaleView.EmployeeID = employeeId;
                currentSaleView.PaymentType = paymentType;
                currentSaleView.SaleDate = DateTime.Now;
                foreach (var d in saledetailCart)
                {
                    SaleDetail detail = new SaleDetail();
                    detail.PartID = d.PartId;
                    detail.Quantity = d.Qty;
                    detail.SellingPrice = d.PartPrice;

                    currentSaleView.SaleDetails.Add(detail);

                    //Parts parttoUpdate = new Parts();
                    Part parttoUpdate = _context.Parts.Where(x => x.PartID == d.PartId).FirstOrDefault();
                    parttoUpdate.QuantityOnHand = (d.QoH - d.Qty);
                    _context.Parts.Update(parttoUpdate);
                }
                //Decimal subTotal = saledetailCart.Sum(x => (x.Qty * x.PartPrice));
                //Console.WriteLine(subTotal);
                //Decimal subTotalwithDiscount = ((subTotal * discountPercentage) / 100);
                //Console.WriteLine(subTotalwithDiscount);
                currentSaleView.SubTotal = subtotal;
                currentSaleView.TaxAmount = tax;

                _context.Sales.Add(currentSaleView);

                _context.SaveChanges();
                int SalesId = _context.Sales.Select(x => x.SaleID).Max();
                return SalesId;
            }
        }
        public async Task<int> GetReturnID()
        {
            int returnID = _context.SaleRefunds.Max(x => x.SaleRefundID);
            return returnID;
        }
        public void GenerateReturnInvoice(ref List<ReturnDetailsView> refundDetails, int SaleId, decimal tax, decimal subtotal)
        {
            if (SaleId == 0)
            {
                errorList.Add(new Exception("Please enter SaleID"));

            }
            else if (refundDetails.Count() == 0)
            {
                errorList.Add(new Exception("No items on sales invoice"));
            }
            else
            {
               
                if (subtotal == 0)
                {
                    errorList.Add(new Exception("You haven't returned any parts"));
                }
                
                SaleRefund currentrefund = new SaleRefund();
                currentrefund.SaleRefundDate = DateTime.Now;
                currentrefund.SaleID = SaleId;
                currentrefund.EmployeeID = 1;
                currentrefund.SubTotal = subtotal;
                currentrefund.TaxAmount = tax;


                foreach (var R in refundDetails)
                {
                    if (R.Qty > 0)
                    {


                        SaleRefundDetail refundDetail = new SaleRefundDetail();
                        refundDetail.PartID = R.PartId;
                        refundDetail.Quantity = R.Qty;
                        refundDetail.SellingPrice = R.Price;
                        refundDetail.Reason = R.Reason;

                        currentrefund.SaleRefundDetails.Add(refundDetail);

                        Part parttoUpdate = _context.Parts.Where(x => x.PartID == R.PartId).FirstOrDefault();

                        parttoUpdate.QuantityOnHand = (parttoUpdate.QuantityOnHand + R.Qty);
                        _context.Parts.Update(parttoUpdate);

                    }
                }

                _context.SaleRefunds.Add(currentrefund);

                if (errorList.Count() > 0)
                {
                    // throw the list of business processing error(s)
                    throw new AggregateException("Unable to generate ReturnInvoice.  Check concerns", errorList);
                }
                else
                {
                    _context.SaveChanges();
                }



            }
            
        }
        public List<ReturnDetailsView> GetReturnCart(int SaleId)
        {
            if (SaleId == 0)
            {
                throw new ArgumentNullException("No SaleId has been entered");
            }
            else
            {
                bool SaleIdExists = _context.Sales.Where(x => x.SaleID == SaleId).Any();
                if (!SaleIdExists)
                {
                    throw new ArgumentNullException("The database has no sale with that SaleID");
                }
                else
                {

                    List<SaleDetail> salesdetails = new List<SaleDetail>();
                    salesdetails=_context.SaleDetails.Where(x => x.SaleID == SaleId).ToList();
                    List<ReturnDetailsView> returns = new List<ReturnDetailsView>();
                    

                    foreach (var d in salesdetails)
                    {
                        string partName = _context.Parts.Where(x => x.PartID == d.PartID).Select(x => x.Description).FirstOrDefault();
                        string refundable = _context.Parts.Where(x => x.PartID == d.PartID).Select(x => x.Refundable).FirstOrDefault();
                        ReturnDetailsView refunddetails = new ReturnDetailsView();
                        refunddetails.PartId = d.PartID;
                        refunddetails.PartName = partName;
                        refunddetails.Price = d.SellingPrice;
                        refunddetails.OrgQty = d.Quantity;
                        refunddetails.Ref = refundable;
                        refunddetails.Reason = string.Empty;
                        refunddetails.RtnQty = _context.SaleRefundDetails.Where(x => x.PartID == d.PartID).Where(x => x.SaleRefund.SaleID == SaleId).Select(x => x.Quantity).FirstOrDefault();
                        refunddetails.Qty = 0;

                        returns.Add(refunddetails);
                    }
                    return returns;
                }
            }


        }
        public async Task<int> ReturnDiscount(int saleId)
        {
            int returnDiscount;
            int? returnDiscountFromSales = _context.Sales.Where(x => x.SaleID == saleId).Select(x => x.CouponID).FirstOrDefault();
            returnDiscount = _context.Coupons.Where(x => x.CouponID == returnDiscountFromSales).Select(x => x.CouponDiscount).FirstOrDefault();

            return returnDiscount;
        }
        Sale sale = new Sale();
        public void removePart(int partId)
        {
            SaleDetail saledetailtoremove = _context.SaleDetails.Where(x => x.PartID == partId).FirstOrDefault();
            sale.SaleDetails.Remove(saledetailtoremove);

        }
        public async Task<SaleDetailView> UpdateCartQty( SaleDetailView salesdetail)
        {
            if(salesdetail.Qty < 0)
            {
                throw new Exception($"{salesdetail.PartName} Qty must be a positive value ");
            }
            else if (salesdetail.QoH < (salesdetail.Qty))
            {
                throw new Exception($"You can't add more quantity of {salesdetail.PartName} then quantity in stock");
            }
            else
            {
                return salesdetail;

            }
          
        }
        public void  UpdateReturnQty(ref List<ReturnDetailsView> returndetail)
        {
            foreach (var returndetails in returndetail)
            {
                if (returndetails.Ref == "N" && returndetails.Qty >0)
                {
                    errorList.Add(new Exception("You cant return non-refundable parts"));
                }
                else if ((returndetails.Qty + returndetails.RtnQty) > returndetails.OrgQty)
                {
                    errorList.Add(new Exception("You cant return more then you bought"));
                }
                else if (returndetails.Qty > 0 && (string.IsNullOrEmpty(returndetails.Reason) || string.IsNullOrWhiteSpace(returndetails.Reason)))
                {
                    errorList.Add(new Exception("You must add a reason for all return Qty's"));
                }
            }
                if (errorList.Count() > 0)
                {
                    // throw the list of business processing error(s)
                    throw new AggregateException("Unable to update part.  Check concerns", errorList);
                }

            
        }
        public async Task<int> validCouponCode(String CouponCode)
        {
            if (String.IsNullOrWhiteSpace(CouponCode))
            {
                throw new ArgumentNullException("Enter a coupon code");
            }
            else
            {
                DateTime startdate = _context.Coupons.Where(x => x.CouponIDValue == CouponCode).Select(x => x.StartDate).FirstOrDefault();
                DateTime enddate = _context.Coupons.Where(x => x.CouponIDValue == CouponCode).Select(x => x.EndDate).FirstOrDefault();
                if (startdate > DateTime.Now || enddate < DateTime.Now)
                {
                    throw new Exception("That coupon is no longer in service");
                }
                else
                {
                    int discount = _context.Coupons.Where(x => x.CouponIDValue == CouponCode)
                .Select(x => x.CouponDiscount).First();

                    return discount;
                }
            }
        }
    }
}

