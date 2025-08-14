
import React, { useState, useEffect } from 'react';
import { csharpApi } from '../../api';
import './HistoryAccountCrud.css';

function HistoryAccountCrud() {
    const [historyAccounts, setHistoryAccounts] = useState([]);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [currentHistoryAccount, setCurrentHistoryAccount] = useState(null);

    useEffect(() => {
        const handleMessage = (event) => {
            const message = event.data;
            if (message.action === 'historyAccountsData') {
                setHistoryAccounts(message.payload);
            }
            if (message.action === 'actionSuccess') {
                setIsModalOpen(false);
                setCurrentHistoryAccount(null);
            }
        };

        csharpApi.addEventListener('message', handleMessage);
        csharpApi.getHistoryAccounts();

        return () => {
            csharpApi.removeEventListener('message', handleMessage);
        };
    }, []);

    const handleAddNew = () => {
        setCurrentHistoryAccount({
            accountId: null,
            action: '',
            timestamp: ''
        });
        setIsModalOpen(true);
    };

    const handleEdit = (historyAccount) => {
        setCurrentHistoryAccount(historyAccount);
        setIsModalOpen(true);
    };

    const handleDelete = (historyAccount) => {
        if (window.confirm(`Are you sure you want to delete history entry ${historyAccount.id}?`)) {
            csharpApi.deleteHistoryAccount(historyAccount);
        }
    };

    const handleSave = (e) => {
        e.preventDefault();
        const historyAccountToSave = {
            ...currentHistoryAccount,
            accountId: parseInt(currentHistoryAccount.accountId, 10) || null
        };
        if (historyAccountToSave.id) {
            csharpApi.updateHistoryAccount(historyAccountToSave);
        } else {
            csharpApi.addHistoryAccount(historyAccountToSave);
        }
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setCurrentHistoryAccount({ ...currentHistoryAccount, [name]: value });
    };

    return (
        <div className="crud-container">
            <div className="card">
                <div className="card-header">
                    <h3>Quản lý Lịch sử Tài khoản</h3>
                    <button className="add-new-btn" onClick={handleAddNew}>+ Thêm mới</button>
                </div>
                <div className="table-container">
                    <table className="crud-table">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Account ID</th>
                                <th>Hành động</th>
                                <th>Thời gian</th>
                                <th className="actions-column">Hành động</th>
                            </tr>
                        </thead>
                        <tbody>
                            {historyAccounts.map((historyAccount) => (
                                <tr key={historyAccount.id}>
                                    <td>{historyAccount.id}</td>
                                    <td>{historyAccount.accountId}</td>
                                    <td>{historyAccount.action}</td>
                                    <td>{new Date(historyAccount.timestamp).toLocaleString()}</td>
                                    <td className="actions-column">
                                        <button className="action-btn edit" onClick={() => handleEdit(historyAccount)}>Sửa</button>
                                        <button className="action-btn delete" onClick={() => handleDelete(historyAccount)}>Xóa</button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>

            {isModalOpen && (
                <div className="modal">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h2>{currentHistoryAccount && currentHistoryAccount.id ? 'Sửa lịch sử tài khoản' : 'Thêm lịch sử tài khoản'}</h2>
                            <button className="close-btn" onClick={() => setIsModalOpen(false)}>&times;</button>
                        </div>
                        <form onSubmit={handleSave}>
                            <div className="form-group">
                                <label>Account ID:</label>
                                <input name="accountId" type="number" value={currentHistoryAccount?.accountId || ''} onChange={handleInputChange} required />
                            </div>
                            <div className="form-group">
                                <label>Hành động:</label>
                                <input name="action" value={currentHistoryAccount?.action || ''} onChange={handleInputChange} required />
                            </div>
                            <div className="form-group">
                                <label>Thời gian:</label>
                                <input name="timestamp" type="datetime-local" value={currentHistoryAccount?.timestamp ? new Date(currentHistoryAccount.timestamp).toISOString().slice(0, 16) : ''} onChange={handleInputChange} required />
                            </div>
                            <div className="form-actions">
                                <button type="submit" className="action-btn save">Lưu</button>
                                <button type="button" className="action-btn cancel" onClick={() => setIsModalOpen(false)}>Hủy</button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}

export default HistoryAccountCrud;
