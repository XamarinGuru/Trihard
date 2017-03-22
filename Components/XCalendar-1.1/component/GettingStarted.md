You can add an `XCalendar` as shown in the following example.

You can also add views via the _Interface Builder_ and wire them up with custom classes.

The former method affords greater control and ease. The lack of seamless toolbox integration of third-party controls in the Interface Builder makes the latter method less desirable.

Example
=======

The following code inserts a calendar.
```
using Softweb.Xamarin.Controls.iOS;
public class XCalendarVC : UIViewController
{
    Calendar calendar;

    public override void ViewDidLoad()
    {
	    base.ViewDidLoad();

        //Create required objects
        var calendar = new Calendar ();
        var menuView = new CalendarMenuView
            { Frame = new CGRect (0f, 0f, 320f, 40f) };
        var contentView = new CalendarContentView 
            { Frame = new CGRect (0f, 45f, 320f, 280f) };
    
        //Customize calendar's appearance
        var appearance = calendar.CalendarAppearance;
        appearance.GetNSCalendar().FirstWeekDay = (nuint)3;
        appearance.DayCircleColorSelected = UIColor.LightGray;
        appearance.DayCircleRatio = (9f / 10f);
        appearance.WeekDayFormat = CalendarWeekDayFormat.Single;
    
        //Link the views to the calendar
        calendar.MenuMonthsView = menuView;
        calendar.ContentView = contentView;
        
	    calendar.DateSelected += DateSelected;
	    calendar.NextPageLoaded += DidLoadNextPage;
	    calendar.PreviousPageLoaded += DidLoadPreviousPage;
	
        // Pass a function that returns text to display in month label
        //E.g. "JAN 2014" OR “01/2014"
        appearance.SetMonthLabelTextCallback (
            //Full month-name and year. E.g. DECEMBER 2014
            (NSDate date, Calendar cal) => new NSString (
                ((DateTime)date).ToString ("MMMM yyyy")));
    
        //Add the views to the current view
        View.Add(menuView);
        View.Add(contentView);
    }

    public override void ViewWillAppear(bool animated)
    {
	    base.ViewWillAppear(animated);

	    //Reload calendar
	    calendar.ReloadData();
    }

    public void DateSelected(object sender, DateSelectedEventArgs args)
    {
    	Console.WriteLine(String.Format("Selected date is {0}", ((DateTime) args.Date).ToLocalTime().ToString("dd-MMM-yyyy")));
    }

    public void DidLoadPreviousPage(object sender, EventArgs args)
    {
    	Console.WriteLine("Loaded previous page");
    }

    public void DidLoadNextPage(object sender, EventArgs args)
    {
    	Console.WriteLine("Loaded next page");
    }
...
}
```
### Adding an Event Viewer
The following code adds an event viewer to your calendar.
```
//Create an EventView instance
var eventView = new CalendarEventView
    { Frame = new CGRect (0f, 370f, 320f, 180f) };

//Assign it to the EventView property of Calendar
calendar.EventView = eventView;

//Add EventView to your main View
View.Add (eventView);

//Remember to specify the DateTimeKind for event date
var fSharpMeetDate =
    new DateTime (2014, 12, 26, 19, 30, 00, DateTimeKind.Local);

//Add events
calendar.EventSchedule = new EventDetails[] {
    new EventDetails (
	    startDate: (NSDate) DateTime.Now,
	    endDate: NSDate.Now.AddSeconds(3000),
	    title: "Writing documentation"),
    new EventDetails (
    	(NSDate) fSharpMeetDate,
	    (NSDate) fSharpMeetDate.AddHours(2),
	    "F# Group Meeting")
};

```

Adding Views Using Interface Builder
====================================

If you want to add layout constraints using the Interface Builder, follow the steps below:

1) Create (empty) subclasses of `CalendarMenuView`, `CalendarContentView`, and `CalendarEventView`.

2) Wire up the `UIView`s you created for `XCalendar` in your layout (storyboard/XIB) with the appropriate classes created in _step 1_ by setting the `Custom Class` property to the class name.

3) In your code, assign the views to the appropriate properties of your `XCalendar` object as shown in the example above.
