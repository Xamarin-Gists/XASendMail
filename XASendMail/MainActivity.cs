using System;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;

using MimeKit;
using MailKit.Net.Smtp;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Java.Lang;
using Android.Widget;

namespace XASendMail
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        TextInputEditText edtFrom, edtTo, edtSubject, edtMessage;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));

            edtFrom = FindViewById<TextInputEditText>(Resource.Id.textInputFrom);
            edtTo = FindViewById<TextInputEditText>(Resource.Id.textInputTo);
            edtSubject = FindViewById<TextInputEditText>(Resource.Id.textInputSubject);
            edtMessage = FindViewById<TextInputEditText>(Resource.Id.textInputMessage);

            FindViewById<FloatingActionButton>(Resource.Id.fab).Click += FabOnClick;
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            new MailAsyncTask(this).Execute();
        }

        class MailAsyncTask : AsyncTask
        {
            string username = "mail-id or username", password = "password", host = "smtp.gmail.com";
            int port = 25;
            MainActivity mainActivity;
            ProgressDialog progressDialog;

            public MailAsyncTask(MainActivity activity)
            {
                mainActivity = activity;
                progressDialog = new ProgressDialog(mainActivity);
                progressDialog.SetMessage("Sending...");
                progressDialog.SetCancelable(false);
            }

            protected override void OnPreExecute()
            {
                base.OnPreExecute();
                progressDialog.Show();
            }

            protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
            {
                try
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("From", mainActivity.edtFrom.Text));
                    message.To.Add(new MailboxAddress("To", mainActivity.edtTo.Text));
                    message.Subject = mainActivity.edtSubject.Text;

                    message.Body = new TextPart("plain")
                    {
                        Text = mainActivity.edtMessage.Text
                    };

                    using (var client = new SmtpClient())
                    {
                        // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                        client.Connect(host, port, false);

                        // Note: only needed if the SMTP server requires authentication
                        client.Authenticate(username, password);
                        client.Send(message);
                        client.Disconnect(true);
                    }
                    return "Successfully Sent";
                }
                catch (System.Exception ex)
                {
                    return ex.Message;
                }
            }

            protected override void OnPostExecute(Java.Lang.Object result)
            {
                base.OnPostExecute(result);
                progressDialog.Dismiss();
                mainActivity.edtFrom.Text = null;
                mainActivity.edtTo.Text = null;
                mainActivity.edtSubject.Text = null;
                mainActivity.edtMessage.Text = null;
                Toast.MakeText(mainActivity, "Email Succesfully Sent...", ToastLength.Short).Show();
            }
        }
    }
}

