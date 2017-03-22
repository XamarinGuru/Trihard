
About XCalendar
===============

XCalendar has a _modern, minimalistic look-and-feel_ coupled with an intuitive API to enable you to quickly start working with it.

XCalendar wraps a native third-party library in a Xamarin binding library that targets the new **Unified API**.

Modular Structure
=================

Views for XCalendar are _loosely coupled_. You have the freedom to add only the views you require. For instance, you can leave out the event viewer and only show the calendar content view.

XCalendar can be added entirely _programmatically_ or using the _Interface Builder_ with custom classes.

XCalendar also supports a **full-month mode** that displays the entire month’s days in the content grid, and a **week mode** that displays only a week’s days. You can set this at design time or provide the ability to the user to dynamically switch between modes.

Event Viewer
============

XCalendar provides an event-viewer identical to the one in iOS’s built-in calendar app. You simply need to pass a **list of events** with their title, start-date, and end-date.

Multiple Calendars
==================

You can add a single calendar or multiple ones inside a view controller and manage them painlessly. Event handler methods provide a **parameter** for the XCalendar that raised the event.

C# Style Events
===============

You can add and remove event handlers using the familiar C# style. XCalendar provides the following events:

- _DateSelected_ - Occurs when a date is selected
- _PreviousPageLoaded_ - Occurs when you swipe to the previous month/week
- _NextPageLoaded_ - Occurs when you swipe to the next month/week

Appearance
==========

XCalendar provides rich APIs you can leverage to fully _**customize** its appearance to match your app’s theme_.

You can change the look and feel of many visual elements based on their state including:
- background colors
- selection circle size
- highlight colors
- text colors

You can also customize the _title text_ that typically depicts the month name and year for the month being viewed.

For instance, you can pass a formatted string to show full text such as “January 2014” or short text such as “01/2014” instead of the default “Jan 2014”.

***

XCalendar wraps Jonathan Tribouharet’s _JTCalendar_ in a Xamarin binding library. It also introduces a feature unavailable in the original _JTCalendar_ — **an event viewer**.