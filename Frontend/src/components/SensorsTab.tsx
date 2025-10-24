import { useEffect, useState } from "react";
import { type RealtimeSensorData, type Sensor } from "../Interfaces/types";
import { getSensorDevices } from "../api/api";
import { realtimeConnection } from "../signalRConnection";
import "../styles/SensorsTab.css";

const SensorsTab = () => {
  const [sensors, setSensors] = useState<Sensor[]>([]);
  const [realtimeData, setRealtimeData] = useState<RealtimeSensorData[]>([]);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  useEffect(() => {
    const getSensors = async () => {
      try {
        setLoading(true);
        const data = await getSensorDevices();
        setSensors(data);
      } catch {
        setError("Kunde inte ladda sensorer");
      } finally {
        setLoading(false);
      }
    };
    getSensors();
  }, []);

  useEffect(() => {
    let isMounted = true;

    const setupRealtime = async () => {
      try {
   
        if (!isMounted) return;
        

        realtimeConnection.on("measurementReceived", (data) => {
          setRealtimeData((prev) => {
            // Om sensorn redan finns, uppdatera dess värde
            const existingIndex = prev.findIndex(
              (item) => item.deviceId === data.deviceId
            );

            if (existingIndex !== -1) {
              const updated = [...prev];
              updated[existingIndex] = data; // ersätt det gamla värdet
              return updated;
            }

            // Annars lägg till den nya sensorn
            return [...prev, data];
          });
        });
        // realtimeConnection.on("alertRaised",(alert)=>{
        //     console.log("Alert recived ", alert);
            
        // })
      } catch (err) {
        console.log("Error setting up realtime connection: ", err);
      }
    };
    setupRealtime();

    //Städa upp när komponenten unmountas
    return () => {
      isMounted = false;
      realtimeConnection.off("measurementReceived");
    };
  }, []);


  return (
    <div className="sensors">
      {sensors.map((s) => {
        const realtime = realtimeData.find((r) => r.deviceId === s.id);

        // Standardformat för värde
        let displayValue = "No data";

        if (realtime) {
          const { type, value } = realtime;

          if (type === "security") {
            displayValue = value === 1 ? "OK" : "Not OK";
          } else if (type === "motion") {
            displayValue = value === 1 ? "Motion detected" : "No motion";
          } else if (typeof value === "number") {
            // avrunda till 1 decimal
            displayValue = `${value.toFixed(1)}`;
          }
        }
        return (
          <div key={s.id} className="sensorDevice">
            <div className="deviceInfo">
            <h3>{s.model}</h3>
            <p>Serial: {s.serial}</p>
            <p>Status: {s.status}</p>
            </div>
            {realtime ? (
              <div className="realtimeData">
                <p>Type: {realtime.type}</p>
                <p>Value: {displayValue}</p>
                <p>
                  Last updated: {new Date(realtime.time).toLocaleTimeString()}
                </p>
              </div>
            ) : (
              <p style={{ color: "gray" }}>No realtime data yet...</p>
            )}
          </div>
        );
      })}
    </div>
  );
};
export default SensorsTab;
