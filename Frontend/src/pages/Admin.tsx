import React, { useState, useEffect } from "react";
import "../styles/admin.css";
import BookingsTab from "../components/BookingsTab";
import UsersTab from "../components/UsersTab";
import ResourcesTab from "../components/ResourcesTab";
import { connection, realtimeConnection } from "../signalRConnection";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import SensorsTab from "../components/SensorsTab";
import { getSensorDevices } from "../api/api";
import type { Sensor } from "../Interfaces/types";

interface AdminProps {
  token: string;
}

const Admin: React.FC<AdminProps> = ({ token }) => {
  const [activeTab, setActiveTab] = useState("bookings");
  const [sensors, setSensors] = useState<Sensor[]>([]);

  useEffect(() => {
    const page = localStorage.getItem("activePage");
    if (page) {
      setActiveTab(page);
    }
    document.body.classList.add("adminBg");
    return () => {
      document.body.classList.remove("adminBg");
    };
  }, []);

    useEffect(() => {
      const getSensors = async () => {
        try {
          const data = await getSensorDevices();
          setSensors(data);
        } catch {
          console.log("Failed to fetch sensors");
        }
      };
      getSensors();
  
    }, []);

  useEffect(() => {
    const tenantId = import.meta.env.VITE_SENSORS_ID;
    const setupRealtime = async () => {
      try {
        while (realtimeConnection.state !== "Connected") {
          console.log("Waiting for realtime connection...");
          await new Promise((res) => setTimeout(res, 500));
        }
        await realtimeConnection.invoke("JoinTenant", "innovia", tenantId);
      } catch (err) {
        console.error("Error joining tenant:", err);
      }
    };
  
    setupRealtime();
  }, []);
  
  useEffect(() => {
    const handler = (update: any) => {
      toast.info(
        `en ${update.resourceName} har blivit bokad på ${update.date} under ${update.timeSlot}`
      );
      console.log(update);
    };
    connection.on("ReceiveBookingUpdate", handler);
    return () => {
      connection.off("ReceiveBookingUpdate", handler);
    };
  }, []);
  useEffect(() => {
    const handler = (alert: any) => {
      console.log(sensors);
      
      const device= sensors.find((s)=>s.id===alert.deviceId)      
      if(alert.type==="motion"){

        toast.info(
          alert.message && `Motion detected from ${device?.model}`
        );
      }else if(alert.type==="temperature") {
        toast.error(
          alert.message && `The temperature exceeds 28 degrees - from ${device?.model}`
        );
      }
      console.log("Alert received:", alert);
    };
  
    realtimeConnection.on("alertRaised", handler);
  
    return () => {
      realtimeConnection.off("alertRaised", handler);
    };
  }, [sensors]);

  useEffect(() => {
    const handler = (update: any) => {
      toast.info(
        `en ${update.resourceName} har blivit Avbokad på ${update.date} under ${update.timeSlot}`
      );
      console.log(update);
    };
    connection.on("ReceiveDeleteBookingUpdate", handler);
    return () => {
      connection.off("ReceiveDeleteBookingUpdate", handler);
    };
  }, []);

  if (!token) {
    return <div>Loading...</div>;
  }

  return (
    <div className="dashboard">
      <ToastContainer
        position="top-right"
        autoClose={5000}
        hideProgressBar={false}
        newestOnTop={false}
        closeOnClick
        rtl={false}
        pauseOnFocusLoss
        draggable
        pauseOnHover
        theme="light"
      />

      <div className="adminHeaderHolder">
        <header className="header">
          <div>
            <h1>Adminpanel</h1>
          </div>
        </header>
      </div>

      <nav className="tabs">
        <button
          className={`tab ${activeTab === "bookings" ? "active" : ""}`}
          onClick={() => {
            setActiveTab("bookings");
            localStorage.setItem("activePage", "bookings");
          }}>
          BOKNINGAR
        </button>
        <button
          className={`tab ${activeTab === "users" ? "active" : ""}`}
          onClick={() => {
            setActiveTab("users");
            localStorage.setItem("activePage", "users");
          }}>
          ANVÄNDARE
        </button>
        <button
          className={`tab ${activeTab === "resources" ? "active" : ""}`}
          onClick={() => {
            setActiveTab("resources");
            localStorage.setItem("activePage", "resources");
          }}>
          RESURSER
        </button>
        <button 
        className={`tab ${activeTab==="sensors"?"active":""}`} 
        onClick={()=>{
          setActiveTab("sensors");
          localStorage.setItem("activePage","sensors");
        }} > SENSORER</button>
      </nav>

      <div className="content">
        {activeTab === "bookings" && <BookingsTab token={token} />}
        {activeTab === "users" && <UsersTab token={token} />}
        {activeTab === "resources" && <ResourcesTab token={token} />}
        {activeTab==="sensors"&& <SensorsTab/>}
      </div>
    </div>
  );
};

export default Admin;
