using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Utils.Enums
{
    public enum SupportActivities : int
    {
        Shopping = 1,
        //Picking up groceries and other essentials(e.g.food, toiletries, household products)

        CollectingPrescriptions = 2,
        //Collecting prescriptions from a local pharmacy

        Errands = 3,
        //Running essential local errands(e.g.posting mail)

        MedicalAppointmentTransport = 4,
        //Providing transport for essential medical appointments(where it is safe to do so)

        DogWalking = 5,
        //Dog walking

        MealPreparation = 6,
        //Delivering a hot / pre-prepared meal

        PhoneCalls_Friendly = 7,
        //Making / receiving phone calls for those in need of a friendly chat

        PhoneCalls_Anxious = 8,
        //Making / receiving phone calls for those who may be anxious

        HomeworkSupport = 9,
        //Homework support for children being home-schooled

        CheckingIn = 10,
        //Reaching out to people in my area to check-in

        Other = 11,
        //I may be able to help with other tasks. Please check with me.

        FaceMask = 12,
        //making a face covering

        WellbeingPackage = 13,
        //Age UK - Collecting and delivering a pre-prepared wellbeing package

        // HLP 
        CommunityConnector = 14
    }
}