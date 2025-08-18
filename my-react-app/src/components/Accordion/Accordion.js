
import React, { useState } from 'react';
import './Accordion.css';

const Accordion = ({ title, children }) => {
    const [isOpen, setIsOpen] = useState(false);

    return (
        <li className={`accordion-item ${isOpen ? 'open' : ''}`}>
            <div className="accordion-header" onClick={() => setIsOpen(!isOpen)}>
                <span className="accordion-title">{title}</span>
                <i className={`fas fa-chevron-down submenu-arrow`}></i>
            </div>
            {isOpen && <ul className="submenu">{children}</ul>}
        </li>
    );
};

export default Accordion;
