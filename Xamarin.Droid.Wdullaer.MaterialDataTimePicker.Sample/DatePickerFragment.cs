﻿namespace Nivaes.MaterialDataTimePicker.Droid
{
    using Android.OS;
    using Android.Util;
    using Android.Views;
    using Android.Widget;
    using AndroidX.Fragment.App;
    using Java.Util;
    using Com.Wdullaer.MaterialDateTimePicker.Date;
    using Android.Graphics;
    using Android.Content;

    public class DatePickerFragment
        : Fragment, DatePickerDialog.IOnDateSetListener
    {
        private TextView dateTextView;
        private CheckBox modeDarkDate;
        private CheckBox modeCustomAccentDate;
        private CheckBox vibrateDate;
        private CheckBox dismissDate;
        private CheckBox titleDate;
        private CheckBox showYearFirst;
        private CheckBox showVersion2;
        private CheckBox switchOrientation;
        private CheckBox limitSelectableDays;
        private CheckBox highlightDays;
        private CheckBox defaultSelection;
        private DatePickerDialog dpd;

        public DatePickerFragment()
        {
            // Required empty public constructor
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.datepicker_layout, container, false);

            // Find our View instances
            dateTextView = view.FindViewById<TextView>(Resource.Id.date_textview);
            Button dateButton = view.FindViewById<Button>(Resource.Id.date_button);
            modeDarkDate = view.FindViewById<CheckBox>(Resource.Id.mode_dark_date);
            modeCustomAccentDate = view.FindViewById<CheckBox>(Resource.Id.mode_custom_accent_date);
            vibrateDate = view.FindViewById<CheckBox>(Resource.Id.vibrate_date);
            dismissDate = view.FindViewById<CheckBox>(Resource.Id.dismiss_date);
            titleDate = view.FindViewById<CheckBox>(Resource.Id.title_date);
            showYearFirst = view.FindViewById<CheckBox>(Resource.Id.show_year_first);
            showVersion2 = view.FindViewById<CheckBox>(Resource.Id.show_version_2);
            switchOrientation = view.FindViewById<CheckBox>(Resource.Id.switch_orientation);
            limitSelectableDays = view.FindViewById<CheckBox>(Resource.Id.limit_dates);
            highlightDays = view.FindViewById<CheckBox>(Resource.Id.highlight_dates);
            defaultSelection = view.FindViewById<CheckBox>(Resource.Id.default_selection);

            view.FindViewById(Resource.Id.original_button).Click += (o, e) =>
            {
                Calendar now = Calendar.Instance;
                new Android.App.DatePickerDialog(
                    base.Activity,
                    (object sender, Android.App.DatePickerDialog.DateSetEventArgs arg) =>
                    {
                        Log.Debug("Orignal", "Got clicked");
                    },
                    now.Get(CalendarField.Year),
                    now.Get(CalendarField.Month),
                    now.Get(CalendarField.DayOfMonth)
                ).Show();
            };

            // Show a datepicker when the dateButton is clicked
            dateButton.Click += (o, e) =>
            {
                Calendar now = Calendar.Instance;

                if (defaultSelection.Checked)
                {
                    now.Add(CalendarField.Date, 7);
                }
                /*
                It is recommended to always create a new instance whenever you need to show a Dialog.
                The sample app is reusing them because it is useful when looking for regressions
                during testing
                 */
                if (dpd == null)
                {
                    dpd = DatePickerDialog.NewInstance(
                            this,
                            now.Get(CalendarField.Year),
                            now.Get(CalendarField.Month),
                            now.Get(CalendarField.DayOfMonth)
                    );
                }
                else
                {
                    dpd.Initialize(
                            this,
                            now.Get(CalendarField.Year),
                            now.Get(CalendarField.Month),
                            now.Get(CalendarField.DayOfMonth)
                    );
                }
                dpd.ThemeDark = modeDarkDate.Checked;
                dpd.Vibrate(vibrateDate.Checked);
                dpd.DismissOnPause(dismissDate.Checked);
                dpd.ShowYearPickerFirst(showYearFirst.Checked);
                dpd.SetVersion(showVersion2.Checked ? DatePickerDialog.Version.Version2 : DatePickerDialog.Version.Version1);
                if (modeCustomAccentDate.Checked)
                {
                    dpd.SetAccentColor("#9C27B0");
                }
                if (titleDate.Checked)
                {
                    dpd.SetTitle("DatePicker Title");
                }
                if (highlightDays.Checked)
                {
                    Calendar date1 = Calendar.Instance;
                    Calendar date2 = Calendar.Instance;
                    date2.Add(CalendarField.WeekOfMonth, -1);
                    Calendar date3 = Calendar.Instance;
                    date3.Add(CalendarField.WeekOfMonth, 1);
                    Calendar[] days = { date1, date2, date3 };
                    dpd.SetHighlightedDays(days);
                }
                if (limitSelectableDays.Checked)
                {
                    Calendar[] days = new Calendar[13];
                    for (int i = -6; i < 7; i++)
                    {
                        Calendar day = Calendar.Instance;
                        day.Add(CalendarField.DayOfMonth, i * 2);
                        days[i + 6] = day;
                    }
                    dpd.SetSelectableDays(days);
                }
                if (switchOrientation.Checked)
                {
                    if (dpd.GetVersion() == DatePickerDialog.Version.Version1)
                    {
                        dpd.SetScrollOrientation(DatePickerDialog.ScrollOrientation.Horizontal);
                    }
                    else
                    {
                        dpd.SetScrollOrientation(DatePickerDialog.ScrollOrientation.Vertical);
                    }
                }

                //dpd.SetOnCancelListener(dialog-> {
                //    Log.Debug("DatePickerDialog", "Dialog was cancelled");
                //    dpd = null;
                //});

                dpd.Show(RequireFragmentManager(), "Datepickerdialog");
            };

            return view;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            dpd = null;
        }

        public override void OnResume()
        {
            base.OnResume();
            DatePickerDialog dpd = (DatePickerDialog)base.FragmentManager.FindFragmentByTag("Datepickerdialog");
            if (dpd != null) dpd.OnDateSetListener = this;
        }

        void DatePickerDialog.IOnDateSetListener.OnDateSet(DatePickerDialog view, int year, int monthOfYear, int dayOfMonth)
        {
            string date = "You picked the following date: " + dayOfMonth + "/" + (++monthOfYear) + "/" + year;
            dateTextView.Text = date;

            dpd = null;
        }
    }
}
