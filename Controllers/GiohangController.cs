using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLBanHang.Models;
using System.Net;
using System.Net.Mail;

namespace QLBanHang.Controllers
{

    public class GiohangController : Controller
    {
        private qlbanhangEntities db = new qlbanhangEntities();
        // GET: Giohang
        public ActionResult Index()
        {
            List<CartItem> giohang = Session["giohang"] as List<CartItem>;
            return View(giohang);
        }
        // khai báo phương thứ thêm sản phẩm vào giỏ hàng
        public RedirectToRouteResult AddToCart(string MaSP)
        {
            if (Session["giohang"] == null)
            {
                Session["giohang"] = new List<CartItem>();
            }
            List<CartItem> giohang = Session["giohang"] as List<CartItem>;
            //kiểm tra sản phẩm khách đang chọn có trong giỏ hàng chưa
            if (giohang.FirstOrDefault(m => m.MaSp == MaSP) == null)
            {
                SanPham sp = db.SanPhams.Find(MaSP);
                CartItem newItem = new CartItem();
                newItem.MaSp = MaSP;
                newItem.TenSP = sp.TenSP;
                newItem.SoLuong = 1;
                newItem.DonGia = Convert.ToDouble(sp.Dongia);
                giohang.Add(newItem);
            }
            else //sản phẩm đã có trong giỏ  hàng  ==> tăng số lượng lên 1
            {
                CartItem cartItem = giohang.FirstOrDefault(m => m.MaSp == MaSP);
                cartItem.SoLuong++;
            }
            Session["giohang"] = giohang;
            return RedirectToAction("Index");
        }
        public RedirectToRouteResult Update(string MaSP, int txtSoluong)
        {
            // Tìm CartItem cần cập nhật số lượng
            List<CartItem> giohang = Session["giohang"] as List<CartItem>;
            CartItem item = giohang.FirstOrDefault(m => m.MaSp == MaSP);
            if (item != null)
            {
                // Cập nhật số lượng
                item.SoLuong = txtSoluong;
                Session["giohang"] = giohang;
            }
            return RedirectToAction("Index");
        }

        public RedirectToRouteResult DelCartItem(string MaSP)
        {
            // Tìm CartItem muốn xóa
            List<CartItem> giohang = Session["giohang"] as List<CartItem>;
            CartItem item = giohang.FirstOrDefault(m => m.MaSp == MaSP);
            if (item != null)
            {
                // Xóa CartItem khỏi giỏ hàng
                giohang.Remove(item);
                Session["giohang"] = giohang;
            }
            return RedirectToAction("Index");
        }

        public ActionResult Order(string Email, string Phone)
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Phone))
            {
                // Handle missing email or phone
                return RedirectToAction("Index", "Home");
            }

            List<CartItem> giohang = Session["giohang"] as List<CartItem>;
            if (giohang != null && giohang.Any())
            {
                try
                {
                    // Construct email message
                    string sMsg = "<html><body><table border='1'><caption>Thông tin đặt hàng</caption><tr><th>STT</th><th>Tên hàng</th><th>Số lượng</th><th>Đơn giá</th><th> Thành tiền</th></tr>";
                    int i = 0;
                    double tongtien = 0;
                    foreach (CartItem item in giohang)
                    {
                        i++;
                        sMsg += "<tr>";
                        sMsg += "<td>" + i.ToString() + "</td>";
                        sMsg += "<td>" + HttpUtility.HtmlEncode(item.TenSP) + "</td>";
                        sMsg += "<td>" + item.SoLuong.ToString() + "</td>";
                        sMsg += "<td>" + item.DonGia.ToString() + "</td>";
                        sMsg += "<td>" + String.Format("{0:#,###}", item.SoLuong * item.DonGia) + "</td>";
                        sMsg += "</tr>";
                        tongtien += item.SoLuong * item.DonGia;
                    }
                    sMsg += "<tr><th colspan='4'>Tổng cộng:</th><td>" + String.Format("{0:#,### vnđ}", tongtien) + "</td></tr></table>";

                    // Send email
                    using (MailMessage mail = new MailMessage("diachimailnguoigui@gmail.com", Email, "Thông tin đơn hàng", sMsg))
                    {
                        using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
                        {
                            client.EnableSsl = true;
                            client.Credentials = new NetworkCredential("diachimailnguoigui", "matkhau");
                            mail.IsBodyHtml = true;
                            client.Send(mail);
                        }
                    }

                    // Clear the shopping cart after successful order
                    Session["giohang"] = null;

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    // Log the exception
                    // For debugging purposes, you can also return the exception message to the user
                    ViewBag.ErrorMessage = "Đã xảy ra lỗi trong quá trình gửi email: " + ex.Message;
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                // Handle empty cart
                return RedirectToAction("Index", "Home");
            }
        }
    }

  }
