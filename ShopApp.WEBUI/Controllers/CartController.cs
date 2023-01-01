using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Abstract;
using ShopApp.WEBUI.Identity;
using ShopApp.WEBUI.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Net;
using Iyzipay;
using Iyzipay.Request;
using Iyzipay.Model;
using shopapp.entity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace ShopApp.WEBUI.Controllers
{
    [Authorize]
    public class CartController:Controller
    {
        private IOrderService _orderservice;
        private ICartService _cartService;
        private UserManager<User> _userManager;
        public CartController(ICartService cartService, UserManager<User> userManager, IOrderService orderservice)
        {
            this._cartService = cartService;
            this._userManager = userManager;
            this._orderservice= orderservice;
        }
        public IActionResult Index()
        {
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));

            return View(new CartModel()
            {
                CartId =  cart.Id,
                CartItems = cart.CartItems.Select(x=> new CartItemModel()
                {
                    CartItemId = x.Id,
                    ProductId = x.ProductId,
                    Name = x.Product.Name,
                    Price = (double)x.Product.Price,
                    ImageUrl = x.Product.ImageUrl,
                    Quantity = x.Quantity
                }).ToList()
            });
        }
        [HttpPost]
        public IActionResult AddToCart(int productId,int quantity)
        {
            var userId = _userManager.GetUserId(User);
            _cartService.AddToCart(userId, productId, quantity);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult DeleteFromCart(int productId)
        {
            var userId = _userManager.GetUserId(User);
            _cartService.DeleteFromCart(userId, productId);
            return RedirectToAction("Index");
        }
        public IActionResult Checkout()
        {
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));

            var orderModel = new OrderModel();
            orderModel.CartModel = new CartModel()
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(x => new CartItemModel()
                {
                    CartItemId = x.Id,
                    ProductId = x.ProductId,
                    Name = x.Product.Name,
                    Price = (double)x.Product.Price,
                    ImageUrl = x.Product.ImageUrl,
                    Quantity = x.Quantity
                }).ToList()
            };
            
            return View(orderModel);
        }
        [HttpPost]
        public IActionResult Checkout(OrderModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);            
                var cart = _cartService.GetCartByUserId(userId);

                model.CartModel = new CartModel()
                {
                    CartId = cart.Id,
                    CartItems = cart.CartItems.Select(x => new CartItemModel()
                    {
                        CartItemId = x.Id,
                        ProductId = x.ProductId,
                        Name = x.Product.Name,
                        Price = (double)x.Product.Price,
                        ImageUrl = x.Product.ImageUrl,
                        Quantity = x.Quantity
                    }).ToList()
                };
                var payment = PaymentProcess(model);
                if (payment.Status == "success")
                {
                    SaveOrder(model, payment, userId);
                    ClearCart(model.CartModel.CartId);
                    return View("Success");
                }
                else
                {
                    var msg = new AlertMessage()
                    {
                        Message = $"{payment.ErrorMessage}",
                        AlertType = "danger"
                    };
                    TempData["Message"] = JsonConvert.SerializeObject(msg);
                }
            }           
            return View(model);
        }
        public IActionResult GetOrders()
        {
            var userId = _userManager.GetUserId(User);
            var orders =_orderservice.GetOrders(userId);

            var orderlistModel = new List<OrderListModel>();
            OrderListModel ordermodel;
            foreach (var order in orders)
            {
                ordermodel = new OrderListModel();

                ordermodel.OrderId = order.Id;
                ordermodel.OrderNumber = order.OrderNumber; 
                ordermodel.OrderDate= order.OrderDate;
                ordermodel.Phone= order.Phone;
                ordermodel.FirstName= order.FirstName;
                ordermodel.LastName= order.LastName;
                ordermodel.Email=order.Email;
                ordermodel.Address= order.Address;
                ordermodel.City= order.City;
                ordermodel.OrderState=order.OrderState;
                ordermodel.PaymentType= order.PaymentType;
                
                ordermodel.OrderItems = order.OrderItems.Select(i=>new OrderItemModel()
                {
                    OrderItemId= i.Id,
                    Name=i.Product.Name,
                    Price =(double)i.Product.Price,
                    Quantity =i.Quantity,
                    ImageUrl=i.Product.ImageUrl
                }).ToList();

                orderlistModel.Add(ordermodel);
            }
            return View("Orders", orderlistModel);
        }

        private void ClearCart(int cartId)
        {
            _cartService.ClearCart(cartId);
        }

        private void SaveOrder(OrderModel model, Payment payment, string userId)
        {
            var order = new Order();
            order.OrderNumber= new Random().Next(111111,999999).ToString();
            order.OrderState = EnumOrderState.completed;
            order.PaymentType = EnumPaymentType.CreditCart;
            order.PaymentId = payment.PaymentId;
            order.ConversationId= payment.ConversationId;
            order.OrderDate = new DateTime();
            order.FirstName = model.FirstName;
            order.LastName = model.LastName;
            order.UserId = userId;
            order.Address= model.Address;
            order.Phone= model.Phone;
            order.Email= model.Email;
            order.City= model.City;
            order.Note = model.Note;

            order.OrderItems = new List<shopapp.entity.OrderItem>();
            foreach (var item in model.CartModel.CartItems)
            {
                var orderItem = new shopapp.entity.OrderItem()
                {
                    Price= item.Price,
                    Quantity= item.Quantity,
                    ProductId= item.ProductId,

                };
                
                order.OrderItems.Add(orderItem);
            }
            _orderservice.Create(order);
        }

        private Payment PaymentProcess(OrderModel model)
        {
            Options options = new Options();
            options.ApiKey = "sandbox-VocFWxlSXXr1vPUXh0C43mcTKF30lC3M";
            options.SecretKey = "sandbox-AHt7mJxbh8c4OUJB6uCtrGX3Qxe7XL0p";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = new Random().Next(111111111, 999999999).ToString();
            request.Price = model.CartModel.TotalPrice().ToString();
            request.PaidPrice = model.CartModel.TotalPrice().ToString();
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = "B67832";
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = model.CartName;
            paymentCard.CardNumber = model.CartNumber;
            paymentCard.ExpireMonth = model.ExpirationMonth;
            paymentCard.ExpireYear = model.ExpirationYear;
            paymentCard.Cvc = model.Cvc;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            //paymentCard.CardNumber = "5528790000000008";
            //paymentCard.ExpireMonth = "12";
            //paymentCard.ExpireYear = "2030";
            //paymentCard.Cvc = "123";

            Buyer buyer = new Buyer();
            buyer.Id = "BY789";
            buyer.Name = model.FirstName;
            buyer.Surname = model.LastName;
            buyer.GsmNumber = "+905350000000";
            buyer.Email = "email@email.com";
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            buyer.Ip = "85.34.78.112";
            buyer.City = "Istanbul";
            buyer.Country = "Turkey";
            buyer.ZipCode = "34732";
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = "Jane Doe";
            shippingAddress.City = "Istanbul";
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = "Jane Doe";
            billingAddress.City = "Istanbul";
            billingAddress.Country = "Turkey";
            billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();
            BasketItem basketItem;

            foreach (var item in model.CartModel.CartItems)
            {
                string urunprice = (item.Price*item.Quantity).ToString();
                string urunfiyat = urunprice.Replace(",", ".");
                basketItem = new BasketItem();
                basketItem.Id = item.ProductId.ToString();
                basketItem.Name = item.Name;
                basketItem.Category1 = "Telefon";
                basketItem.Price = urunfiyat;
                basketItem.ItemType = BasketItemType.PHYSICAL.ToString();

                basketItems.Add(basketItem);
            }
           
            request.BasketItems = basketItems;

            return Payment.Create(request, options);
            
        }
    }
}
