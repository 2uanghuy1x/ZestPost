import React, { useState, useRef, useEffect } from 'react';
import './Accordion.css';

const Accordion = ({ title, children }) => {
    const [isOpen, setIsOpen] = useState(false);
    const contentRef = useRef(null);

    useEffect(() => {
        if (contentRef.current) {
            contentRef.current.style.maxHeight = isOpen ? `${contentRef.current.scrollHeight}px` : '0px';
        }
    }, [isOpen]);

    return (
        <div className={`accordion-item ${isOpen ? 'active' : ''}`}>
            <div className="accordion-title" onClick={() => setIsOpen(!isOpen)}>
                <span>{title}</span>
                <span className="accordion-icon">{isOpen ? '▲' : '▼'}</span>
            </div>
            <div
                ref={contentRef}
                className="accordion-content"
            >
                {children}
            </div>
        </div>
    );
};

export default Accordion;
