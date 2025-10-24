import * as signalR from "@microsoft/signalr";

const token = localStorage.getItem("token");

//Connection till BookingHub
export const connection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5022/bookingHub", {
    accessTokenFactory: () => token || ""
  })
  .withAutomaticReconnect()
  .build();

//Connection till Realtimehub
export const realtimeConnection= new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5103/hub/telemetry")
  .withAutomaticReconnect()
  .build()