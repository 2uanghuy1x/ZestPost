import React, { useState, useEffect } from 'react';
import { csharpApi } from './api';

function AccountCrud() {
    const [accounts, setAccounts] = useState([]);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [currentAccount, setCurrentAccount] = useState(null);

    useEffect(() => {
        const handleMessage = (event) => {
            const message = event.data;
            if (message.action === 'accountsData') {
                setAccounts(message.payload);
            }
            if (message.action === 'accountActionSuccess') {
                csharpApi.getAccounts();
                setIsModalOpen(false);
                setCurrentAccount(null);
            }
            if (message.action === 'error') {
                alert('An error occurred in C#: ' + message.payload);
            }
        };

        csharpApi.addEventListener('message', handleMessage);
        csharpApi.getAccounts();

        return () => {
            csharpApi.removeEventListener('message', handleMessage);
        };
    }, []);

    const handleAddNew = () => {
        setCurrentAccount({ uid: '', name: '', email: '', passmail: '', status: 'Live', note: '' });
        setIsModalOpen(true);
    };

    const handleEdit = (account) => {
        setCurrentAccount(account);
        setIsModalOpen(true);
    };

    const handleDelete = (account) => {
        if (window.confirm(`Are you sure you want to delete account ${account.name} (${account.uid})?`)) {
            csharpApi.deleteAccount(account);
        }
    };

    const handleSave = (e) => {
        e.preventDefault();
        if (currentAccount.id) {
            csharpApi.updateAccount(currentAccount);
        } else {
            csharpApi.addAccount(currentAccount);
        }
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setCurrentAccount({ ...currentAccount, [name]: value });
    };

    return (
        <div className="content-container">
            <div className="content-header">
                <h1>Quản lý Tài khoản</h1>
                <button className="add-new-btn" onClick={handleAddNew}>+ Thêm mới</button>
            </div>
            <table>
                <thead>
                    <tr>
                        <th>UID</th>
                        <th>Tên</th>
                        <th>Email</th>
                        <th>Trạng thái</th>
                        <th>Ghi chú</th>
                        <th className="actions-column">Hành động</th>
                    </tr>
                </thead>
                <tbody>
                    {accounts.map((account) => (
                        <tr key={account.id}>
                            <td>{account.uid}</td>
                            <td>{account.name}</td>
                            <td>{account.email}</td>
                            <td>{account.status}</td>
                            <td>{account.note}</td>
                            <td>
                                <button className="action-btn edit" onClick={() => handleEdit(account)}>Sửa</button>
                                <button className="action-btn delete" onClick={() => handleDelete(account)}>Xóa</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            {isModalOpen && (
                <div className="modal">
                    <div className="modal-content">
                        <h2>{currentAccount && currentAccount.id ? 'Sửa tài khoản' : 'Thêm tài khoản'}</h2>
                        <form onSubmit={handleSave}>
                            <label>UID:</label>
                            <input name="uid" value={currentAccount ? currentAccount.uid : ''} onChange={handleInputChange} required />
                            
                            <label>Tên:</label>
                            <input name="name" value={currentAccount ? currentAccount.name : ''} onChange={handleInputChange} />

                            <label>Email:</label>
                            <input type="email" name="email" value={currentAccount ? currentAccount.email : ''} onChange={handleInputChange} />
                            
                            <label>Password Mail:</label>
                            <input name="passmail" value={currentAccount ? currentAccount.passmail : ''} onChange={handleInputChange} />

                            <label>Trạng thái:</label>
                            <input name="status" value={currentAccount ? currentAccount.status : ''} onChange={handleInputChange} />

                            <label>Ghi chú:</label>
                            <textarea name="note" value={currentAccount ? currentAccount.note : ''} onChange={handleInputChange}></textarea>
                            
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

export default AccountCrud;
