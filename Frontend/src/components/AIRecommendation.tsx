import { useEffect, useState } from "react";
import { createBooking, getBookingRecommendation } from "../api/api";
import "../styles/AIRecommendation.css";
import resourceData from "../data/resourceData";

type Recommendation = {
  resourceType: string;
  resourceName: string;
  resourceTypeId: number;
  date: string;
  timeSlot: string;
};

const AIRecommendation = () => {
  const [errorMessage, setErrorMessage] = useState("");
  const [loading, setLoading] = useState(false);
  const [recommendation, setRecommendation] = useState<Recommendation | null>(null);
  const [hasBooked, setHasBooked] = useState(false);
  const [resourceImg, setResourceImg] = useState("");
  const storedName = localStorage.getItem("userName");

  //Rekommendation
  const fetchRecommendation = async () => {
    setErrorMessage("");
    setLoading(true);
    setHasBooked(false);

    const token = localStorage.getItem("token");
    if (!token) {
      setErrorMessage("Logga in igen");
      setLoading(false);
      return;
    }

    //Gör anrop till api
    try {
      const data = await getBookingRecommendation(token);
      setRecommendation(data);
    } catch (error: any) {
      setRecommendation(null);
      setErrorMessage("Något gick fel vid hämtning av rekommendationen.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchRecommendation();
  }, []);

  //Bokning
  const handleBook = async () => {
    const token = localStorage.getItem("token");
    const userId = localStorage.getItem("userId");
    if (!userId || !recommendation || !token) {
      return;
    }
    setLoading(true);
    setErrorMessage("");

    //Gör anrop till api
    try {
      await createBooking(
        {
          date: recommendation.date,
          timeSlot: recommendation.timeSlot,
          resourceTypeId: recommendation.resourceTypeId,
          userId,
        },
        token
      );

      setHasBooked(true);
      
    } catch (error: any) {
      if (error?.response?.status === 409) {
        setErrorMessage("Denna tid är redan bokad. Välj en annan tid.");
      } else {
        setErrorMessage("Något gick fel vid bokningen. Försök igen.");
      }
    } finally {
      setLoading(false);
    }
  };

  //Hämta bild som motsvarar resourcetype
  useEffect(() => {
    const rd = resourceData.find(
      (r) => r.name.toLowerCase() === recommendation?.resourceType.toLowerCase()
    );

    if (rd) {
      setResourceImg(rd.imageUrl);
    } else {
      setResourceImg("");
    }
  }, [recommendation]);

  //RENDER
  return (
    <div className="recommendationHolder">
      <div className="recommendation">
        {hasBooked ? (
          <div className="confirmation">
            <h3>Tack för din bokning!</h3>
            <button
              onClick={fetchRecommendation}
              className="formBtn reserveBtn"
            >
              Få en ny rekommendation
            </button>
          </div>
        ) : (
          <>
            {errorMessage && <p>{errorMessage}</p>}
            {loading && <p>Laddar rekommendation...</p>}
            {recommendation && (
              <div>
                <h3>Hej {storedName}, vill du boka:</h3>
                <div className="recommendationInfoHolder">
                  {resourceImg.length > 1 && (
                    <img src={resourceImg} alt={recommendation.resourceType} />
                  )}
                  <div className="recommendationInfo">
                    <p className="resourceType">
                      {recommendation.resourceType}
                    </p>
                    <p>{recommendation.timeSlot}</p>
                    <p>{recommendation.date}</p>
                    <button onClick={handleBook} className="formBtn reserveBtn">
                      Boka
                    </button>
                  </div>
                </div>
              </div>
            )}
          </>
        )}
      </div>
    </div>
  );
};
export default AIRecommendation;
