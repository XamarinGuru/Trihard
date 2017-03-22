
using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.IO;
using Android.Graphics;
using Android.Provider;
using PortableLibrary;
using GalaSoft.MvvmLight.Helpers;

namespace goheja
{
    public class FragmentProfile : Android.Support.V4.App.Fragment
    {
        ImageView imgProfile;
		TextView lblUsername, lblEmail, lblPhone;

        byte[] bitmapByteData = { 0 };

		RootMemberModel MemberModel { get; set; }
		SwipeTabActivity rootActivity;

		View mView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			MemberModel = new RootMemberModel();
			rootActivity = this.Activity as SwipeTabActivity;

			return inflater.Inflate(Resource.Layout.fProfile, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

			mView = view;

			if (!rootActivity.IsNetEnable()) return;

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				//rootActivity.ShowLoadingView(Constants.MSG_LOADING_USER_DATA);

				MemberModel.rootMember = rootActivity.GetUserObject();

				//rootActivity.HideLoadingView();

				rootActivity.RunOnUiThread(() =>
				{
					SetUIVariablesAndActions();
					SetInputBinding();
				});
			});
        }

		private void SetUIVariablesAndActions()
		{
			imgProfile = mView.FindViewById<ImageView>(Resource.Id.imgProfile);
			lblUsername = mView.FindViewById<TextView>(Resource.Id.lblUsername);
			lblEmail = mView.FindViewById<TextView>(Resource.Id.lblEmail);
			lblPhone = mView.FindViewById<TextView>(Resource.Id.lblPhone);

			imgProfile.Click += ActionChangePicture;
			mView.FindViewById<LinearLayout>(Resource.Id.ActionEditProfile).Click += ActionEditProfile;
			mView.FindViewById<LinearLayout>(Resource.Id.ActionSyncDevice).Click += ActionSyncDevice;
			mView.FindViewById<LinearLayout>(Resource.Id.ActionChangePassword).Click += ActionChangePassword;
			mView.FindViewById<LinearLayout>(Resource.Id.ActionSignOut).Click += ActionSignOut;
		}

		private void SetInputBinding()
		{
			if (MemberModel.rootMember == null) return;

			try
			{
				this.SetBinding(() => MemberModel.username, () => lblUsername.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.email, () => lblEmail.Text, BindingMode.TwoWay);
				this.SetBinding(() => MemberModel.phone, () => lblPhone.Text, BindingMode.TwoWay);

				SetBitmapImg();
			}
			catch (Exception err)
			{
				Toast.MakeText(Activity, err.ToString(), ToastLength.Long).Show();
			}
		}

		private void ActionChangePicture(object sender, EventArgs e)
		{
			if (!rootActivity.IsNetEnable()) return;

			var imageIntent = new Intent();
			imageIntent.SetType("image/*");
			imageIntent.SetAction(Intent.ActionGetContent);
			StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), 0);
		}

        private void ActionEditProfile(object sender, EventArgs e)
        {
			if (!rootActivity.IsNetEnable()) return;

			var intent = new Intent(Activity, typeof(EditProfileActivity));
			StartActivityForResult(intent, 1);
        }

		private void ActionSyncDevice(object sender, EventArgs e)
		{
			if (!rootActivity.IsNetEnable()) return;

			var userID = rootActivity.GetUserID();
			var uri = Android.Net.Uri.Parse(string.Format(Constants.URL_WATCH, userID, Constants.SPEC_GROUP_TYPE));
			var intent = new Intent(Intent.ActionView, uri);
			StartActivityForResult(intent, 1);
		}

		private void ActionChangePassword(object sender, EventArgs e)
		{
			if (!rootActivity.IsNetEnable()) return;

			var intent = new Intent(Activity, typeof(ChangePasswordActivity));
			AppSettings.currentEmail = MemberModel.email;
			StartActivityForResult(intent, 1);
		}

		private void ActionSignOut(object sender, EventArgs e)
		{
			rootActivity.SignOutUser();

			var activity = new Intent(this.Activity, typeof(LoginActivity));
			StartActivity(activity);
			rootActivity.Finish();
		}

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
			try
			{
				if (resultCode == (int)Result.Ok)
				{
					Bitmap mewbm = NGetBitmap(data.Data);

					Bitmap newBitmap = scaleDown(mewbm, 200, true);
					using (var stream = new MemoryStream())
					{
						newBitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
						bitmapByteData = stream.ToArray();
					}
					ExportBitmapAsPNG(GetRoundedCornerBitmap(newBitmap, 400));

					rootActivity.SaveUserImage(bitmapByteData);
				}
			}
			catch (Exception err)
			{
				Toast.MakeText(Activity, err.ToString(), ToastLength.Long).Show();
			}
        }

        void ExportBitmapAsPNG(Bitmap bitmap)
        {
            try
            {
                var sdCardPath = Android.OS.Environment.DataDirectory.AbsolutePath;
				var filePath = System.IO.Path.Combine(sdCardPath, Constants.PATH_USER_IMAGE);
                var stream = new FileStream(filePath, FileMode.Create);

                bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);// Bitmap.CompressFormat.Png, 100, stream);
                stream.Close();
                var s2 = new FileStream(filePath, FileMode.Open);

                Bitmap bitmap2 = BitmapFactory.DecodeFile(filePath);
                imgProfile.SetImageBitmap(bitmap2);
                s2.Close();
                GC.Collect();
            }
            catch (Exception err)
            {
                Toast.MakeText(Activity, err.ToString(), ToastLength.Long).Show();
            }
        }
        private Bitmap NGetBitmap(Android.Net.Uri uriImage)
        {
            Bitmap mBitmap = null;
            mBitmap = MediaStore.Images.Media.GetBitmap(Activity.ContentResolver, uriImage);
            return mBitmap;
        }
        private static Bitmap GetRoundedCornerBitmap(Bitmap bitmap, int pixels)
        {
            Bitmap output = null;

            try
            {
                output = Bitmap.CreateBitmap(bitmap.Width, bitmap.Height, Bitmap.Config.Argb8888);
                Canvas canvas = new Canvas(output);

                Color color = new Color(66, 66, 66);
                Paint paint = new Paint();
                Rect rect = new Rect(0, 0, bitmap.Width * 5 / 5, bitmap.Height * 5 / 5);
                RectF rectF = new RectF(rect);
                float roundPx = pixels;

                paint.AntiAlias = true;
                canvas.DrawARGB(0, 0, 0, 0);
                paint.Color = color;
                canvas.DrawRoundRect(rectF, roundPx, roundPx, paint);

                paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
                canvas.DrawBitmap(bitmap, rect, rect, paint);
            }
            catch
            {
				return null;
            }

            return output;
        }

        public static Bitmap scaleDown(Bitmap realImage, float maxImageSize, bool filter)
        {
            float ratio = Math.Min((float)maxImageSize / realImage.Width, (float)maxImageSize / realImage.Width);
            int width = (int)Math.Round((float)ratio * realImage.Width);
            int height = (int)Math.Round((float)ratio * realImage.Width);

            Bitmap newBitmap = Bitmap.CreateScaledBitmap(realImage, width, height, filter);
            return newBitmap;
        }
        void SetBitmapImg()
        {
			try
			{
				var sdCardPath = Android.OS.Environment.DataDirectory.AbsolutePath;
				var filePath = System.IO.Path.Combine(sdCardPath, Constants.PATH_USER_IMAGE);
				var s2 = new FileStream(filePath, FileMode.Open);
				Bitmap bitmap2 = BitmapFactory.DecodeFile(filePath);
				imgProfile.SetImageBitmap(bitmap2);
				s2.Close();
			}
			catch (Exception err)
			{
				//Toast.MakeText(Activity, err.ToString(), ToastLength.Long).Show();
			}
        }
    }
}
