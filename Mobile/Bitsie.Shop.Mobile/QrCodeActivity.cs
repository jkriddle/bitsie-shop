using ZXing;
using Android.Graphics;
using ZXing.Common;
using Bitsie.Shop.Infrastructure;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace Bitsie.Shop.Mobile
{
	public class QrCodeActivity : BaseActivity
	{
		protected void GenerateQrCode(ImageView imageview, string paymentUrl) {
			var writer = new ZXing.QrCode.QRCodeWriter();

			int width0 = 600;
			int height0 = 600;

			int colorBack = Color.Black;
			int colorFront = Color.White;

			try
			{
				IDictionary<EncodeHintType, Object> hint = new Dictionary<EncodeHintType, Object>();
				hint.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
				BitMatrix bitMatrix = writer.encode(paymentUrl, ZXing.BarcodeFormat.QR_CODE, width0, height0, hint);
				int width = bitMatrix.Width;
				int height = bitMatrix.Height;
				int[] pixels = new int[width * height];
				for (int y = 0; y < height; y++)
				{
					int offset = y * width;
					for (int x = 0; x < width; x++)
					{

						pixels[offset + x] = bitMatrix[x, y] ? colorBack : colorFront;
					}
				}

				Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
				bitmap.SetPixels(pixels, 0, width, 0, 0, width, height);
				imageview.SetImageBitmap(bitmap);
			} catch (WriterException e) {
				Console.WriteLine (e.Message);
			}
		}
	}
}

